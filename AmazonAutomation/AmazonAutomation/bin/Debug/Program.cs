/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//Name :Abhishek Jayawantrao Patil 
//    last edit :8/10/2021 12:00PM
//_________________________________________________________________________________________________________________
//   Assignment 2
//          1.Launch Calculator
//          2.Do some operations based on your choice
//          3.Take snapshots at each operation, Copy them to a network location and close calculator
//      
//  Steps :
//          1.Getting 2 inputs as a two numbers and a operation from User.
//          2.Calculator Opening. Performing Opertion on given inputs. Taking ScreenShot at each Action. 
//            Storing screenshots in Network Location. 
//
//  Process:  
//            1. Check calculator is open or not
//                    2. if open, excute on that if not opened then open
//                    3. Take inputs from user : A. fist Number    B.Second Number     C.Opertaion sign
//                    4. Convert those inputs in strings so that we can use it in naming
//            5. Check whether inputs are proper or not 
//                    6.if valid proceed further ,if not give exception and stops
//                            7.If open, Declare hierarchy wise... declaration of  AE_Calc --> AE_group ---> AE_Number_pad ---> {Declrare AE_input1 & AE_input2 & AE_operation} Scope wise*
//                            8. Scope wise press the buttons and check it whether it is pressed or not
//                            9.if pressed TOOK ScreenShot and stored at Network location
//                            10.Check whether there are any exceptions or errors or not   Act accordingly give exceptions
//            11.Check Screen shots are taken or not 
//            12.Check whether operations are performed or not
//            13 Check for Success wait for 4 sec (sleep function ) then END !
//
//_________________________________________________________________________________________________________________


using System;
using Nvidia.AtpLib;
using System.Windows.Automation;
using System.Threading;

namespace Assignment2
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                int i = 1, j = 1;
                bool Check_program = false;
                bool is_Cal_already_opened = false;
                string str_continue;
                UIA uia = new UIA();
                Program obj_pro1 = new Program();

                try
                {
                    System.Diagnostics.Process[] C = System.Diagnostics.Process.GetProcessesByName("Calculator");
                    if (C.Length != 0)
                    {
                        Console.WriteLine(i++ + ".Calculator already Opened.");
                        is_Cal_already_opened = true;
                        AutomationElement AE_Calc = uia.GetElementByControlTypeAndName(AutomationElement.RootElement, ControlType.Window, "Calculator", true, 60);
                        Check_program = Assignment2_Helper.All_calcultor_Operations(uia, i, j, AE_Calc, is_Cal_already_opened);
                    }
                    else
                    {
                        Console.WriteLine(i++ + ".not Opened.");
                        System.Diagnostics.Process.Start("calc.exe");
                        AutomationElement AE_Calc = uia.GetElementByControlTypeAndName(AutomationElement.RootElement, ControlType.Window, "Calculator", true, 60);
                        Console.WriteLine(i++ + ".Calculator Opened.");
                        Check_program = Assignment2_Helper.All_calcultor_Operations(uia, i, j, AE_Calc, is_Cal_already_opened);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    Assignment2_Helper.IsSucecssfunction(Check_program);
                    Console.WriteLine("YOU WANT TO CONTINUE(y/n)!!");                    
                    str_continue = Console.ReadLine();
                    while(str_continue.ToLower() != "y" && str_continue.ToLower() != "n")
                    {
                        Console.Write("The value must be of integer type, try again:\n ");
                        str_continue = Console.ReadLine();                        
                    }
                    
                }
                if (str_continue.ToUpper() == "N")
                {
                    break;
                }
            }

        }
     }
}

