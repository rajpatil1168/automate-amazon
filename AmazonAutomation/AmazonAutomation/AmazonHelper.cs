using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using System.Threading;

namespace AmazonAutomation
{
    class AmazonHelper
    {
        public static void IsSucecssfunction(bool issucess)
        {
            if (issucess)
                Console.WriteLine("PROGRAM SUCCESSFULLY EXECUTED\n\n");
            else
                Console.WriteLine("\n PROGRAM NOT* SUCCESSFULLY EXECUTED\n\n");
        }

        public static bool OpenUrl(string url)
        {
            try
            {
                Global.driver.Navigate().GoToUrl(url);
                Thread.Sleep(1000);
                if (Global.driver.PageSource.Contains("nav-logo-sprites"))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("FAILURE::URL did not load/valid: " + Global.test_url);
                return false;
            }
        }

        public static bool AmazonLogin()
        {
            try
            {
                string email = "9423272";
                string password = "Ready2@";
                ClickIfPresent(By.Id("nav-link-accountList"));
                Global.driver.SwitchTo().Window(Global.driver.WindowHandles.Last());
                SendKeysIfPresent(By.Id("ap_email"), email);
                SendKeysIfPresent(By.Id("ap_email"), email);
                SendKeysIfPresent(By.Id("ap_email"), email);
                ClickIfPresent(By.Id("continue"));
                Global.driver.SwitchTo().Window(Global.driver.WindowHandles.Last());
                SendKeysIfPresent(By.Id("ap_password"),password);                
                ClickIfPresent(By.Id("signInSubmit"));
                Thread.Sleep(500);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("FAILURE::AmazonLogin did not load.:{0}",e);
                return false;
            }
        }

        public static bool IsElementPresent(By by)
        {
            try
            {
                if (Global.driver.FindElement(by) != null)
                {
                    ClickIfPresent(By.Id("continue"));
                    Thread.Sleep(1000);
                    return true;
                }
                else
                {
                    Console.WriteLine("IsELEMENT={0}   -->>> is NULL !", by);
                    throw new TestException("IsElementPresent did not load");
                }

            }
            catch (NoSuchElementException)
            {
                throw new TestException("IsElementPresent did not load");
            }
        }

        public static void ClickIfPresent(By by)
        {
            try
            {
                if (IsElementPresent(by))
                {
                    Global.driver.FindElement(by).Click();
                    Console.WriteLine("{0} Clicked !!", by.ToString());
                    Thread.Sleep(1000);
                }
                else
                {
                    Console.WriteLine("NOT CLICKED {0} ", by);
                    Thread.Sleep(1000);
                    ClickIfPresent(By.Id("continue"));
                    throw new TestException("ClickIfPresent did not load");
                }
            }
            catch
            {
                throw new TestException("ClickIfPresent did not load");
            }
        }

        public static void SendKeysIfPresent(By by, string sendkeys)
        {
            try
            {
                if (IsElementPresent(by))
                {
                    Global.driver.FindElement(by).Clear();
                    Thread.Sleep(1000);
                    ClickIfPresent(By.Id("continue"));

                    Global.driver.FindElement(by).SendKeys(sendkeys);
                    Thread.Sleep(500);
                    Console.WriteLine("Send Keys--> {0} to {1} ", sendkeys, by);
                }
                else
                {
                    Console.WriteLine("NOT CLICKED {0} ", by);
                    throw new TestException("SendKeysIfPresent did not load");
                }
            }
            catch
            {
                throw new TestException("SendKeysIfPresent did not load");
            }
        }

        public static void ImplictSleeptrycatch( int time)
        {
            try
            {
                Global.driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(time);
            }
            catch (Exception e)
            {
                Console.WriteLine("FAILURE::Sleep function did not load: ");
            }
        }

        /// <summary>
        /// this function accepts the product names fro user.
        /// </summary>
        /// <returns></returns>
        public static List<string> GetproductNames()
        {
            try
            {
                List<string> productlistName = new List<string>();

                Console.WriteLine("________________________________________________________________________\nInsert the Number of Product (Enter the integer which greater than or equal to{0})", Global.lowerlimit);
                int numberOfinputs;
                while (!int.TryParse(Console.ReadLine(), out numberOfinputs))
                    Console.Write("The value must be of integer type, try again:\n");

                while (numberOfinputs < Global.lowerlimit)
                {
                    Console.WriteLine("Please enter another integer which is greater than {0}: ", Global.lowerlimit);
                    while (!int.TryParse(Console.ReadLine(), out numberOfinputs))
                        Console.Write("The value must be of integer type, try again:\n");
                }

                Console.WriteLine("_______________________________________________________________________\n\nInsert the{0} PRODUCT NAMES to list:", numberOfinputs);
                for (int i = 0; i < numberOfinputs; i++)
                {
                    string input = Console.ReadLine();
                    if (String.Equals(input, string.Empty))
                    {
                        Console.WriteLine("\nPlease insert the \"NON-NULL\" PRODUCT NAMES to list:");
                        numberOfinputs++;
                        continue;
                    }
                    productlistName.Add(input);
                }
                Console.WriteLine();
                Console.WriteLine();
                return productlistName;
            }
            catch (Exception e)
            {
                Console.WriteLine("\n" + e);
                return null;
            }
        }

        public static void CartItemsToList( List<string> idListinPrevPass)
        {
            try
            {
                Global.driver.SwitchTo().Window(Global.driver.WindowHandles.Last());
                ClickIfPresent(By.Id("nav-cart"));
                Thread.Sleep(2000);
                
                string cartTotalAmountXpath = "//*[@id='sc-subtotal-amount-activecart']/span";
                string cartTotaProductXpath = "//span[@id='sc-subtotal-label-buybox']";
                if (IsElementPresent(By.XPath(cartTotalAmountXpath)))
                {
                    //do if exists
                    Console.WriteLine("\n______________________________________________________________________\n");
                    Console.WriteLine("Total Number of Product in cart: " + Global.driver.FindElement(By.XPath(cartTotaProductXpath)).Text);
                    Console.WriteLine("\nTotal amount in cart: " + Global.driver.FindElement(By.XPath(cartTotalAmountXpath)).Text);
                    Console.WriteLine("\n______________________________________________________________________\n");
                }
                else
                {
                    //do if does not exists
                    Console.WriteLine("Total amount in cart: NULL* !");
                }

                string xpathNoRows = "  //div[@data-asin  and @data-minquantity='1']";
                string classNameCart = "a-truncate-cut";
                if (IsElementPresent(By.XPath(xpathNoRows)))
                {
                    List<IWebElement> ListitemsNames = new List<IWebElement>();
                    List<string> productInCartStoredIdList = new List<string>();
                    if (IsElementPresent(By.ClassName(classNameCart)))
                    {
                        ListitemsNames = Global.driver.FindElements(By.ClassName(classNameCart)).ToList();  //Consist Truncate Names of prodcucts in Cart
                    }
                    else
                    {
                        Console.WriteLine("IsElementPresent {0} did not load", By.ClassName(classNameCart).ToString());
                        throw new TestException("IsElementPresent {0} did not load");
                    }
                    Console.WriteLine("\n\n\n______________________________________________________________________\n");

                    foreach (IWebElement listItem in ListitemsNames)
                    {
                        Console.WriteLine("\n Cart Item name: " + listItem.Text + ".!!!!! ");
                    }

                    Console.WriteLine("\n\n\n______________________________________________________________________\n");
                    Console.WriteLine("______________________________________________________________________\n");

                    List<IWebElement> ListTotal = new List<IWebElement>();
                    ListTotal = Global.driver.FindElements(By.XPath(xpathNoRows)).ToList();
                    Console.WriteLine("Cart Item Count( ListTotal.Count): " + ListTotal.Count + "___ListitemsNames.Count: " + ListitemsNames.Count + "___");

                //Info:
                    //idListinPrevPass ===>Entered products IDs in same pass
                    //ListTotal ===> Object of Cart's products
                    //productInCartStoredIdList ===> Cart's Product Ids==> 'data - asin - Id' attribute's list of ListItems object
                    //ListitemsNames ===> string product truncated names

                    foreach (IWebElement listItem in ListTotal)
                    {
                        productInCartStoredIdList.Add(listItem.GetAttribute("data-asin"));          //store ID of Truncate Name of Product in Cart
                    }

                    //Checking previously added products are present in Cart or not !
                    bool results = idListinPrevPass.All(j => productInCartStoredIdList.Contains(j));
                    if (results)
                    {
                        Console.WriteLine("\n{CROSS CHECKING : IS THERE ELEMENT OF PREVIOUS PASS PRESENT IN CART** OR NOT: TRUE/PRESENT}");

                    }
                    else
                    {
                        Console.WriteLine("\n{CROSS CHECKING : ELEMENT OF PREVIOUS PASS is PRESENT IN CART** OR NOT: False/Not** PRESENT}");
                    }
                    Console.WriteLine("\n\n\n______________________________________________________________________\n");
                    Console.WriteLine("______________________________________________________________________\n");

                }
                else
                {
                    //do if does not exists
                    Console.WriteLine("\n Cart Item UNIQUE CODE: Is not Assigned");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("\n" + e + "****ERROR in CART FUNCTON");
            }
        }
    }
}
