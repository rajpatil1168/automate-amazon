using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nvidia.AtpLib;
using IntegrationLib.Game;


namespace GamePlayRun
{
    public class OverWatch : IGamePlay
    {
        private string gameName = "OverWatch";
        private Log log = null;
        //GameID gameID = GameID.OverWatch;
        //PCSteamHelper steam_instance;
        //SteamID.SteamAccount steamAccount;
        OCRHelper ocrHelper;

        public enum OverWatchPage
        {
            LoadingPage,
            StartPage,
            ContinueSettingsPage,
            OptionsPage,
            GameSettingsPage,
            VideoSettingsPage,
            BasicSettingsPage,
            AdvanceSettingsPage,
            GameStartPage,
            GamePausedPage
        }

        public OverWatch(Log _log)
        {
            if (_log == null)
                log = new Log(AtpEnvironment.DebugTaskResultDir + @"\OverWatch.log", true, true);
            else
                log = _log;
            //steamAccount = new SteamID.SteamAccount();
            //steamAccount.ID = System.Configuration.ConfigurationManager.AppSettings["SteamId"];
            //steamAccount.Password = System.Configuration.ConfigurationManager.AppSettings["SteamPwd"];

            /// ----->Battlenet -----!!!
            ocrHelper = new OCRHelper(log);
        }

        public bool ExitGame()
        {
            if (!steam_instance.KillGame(gameID))
            {
                log.WriteLine("Failed to close game", LogType.ERROR);
                return false;
            }
            log.WriteLine("Game closed", LogType.INFO);
            return true;
        }

        public bool IsGameInstalled()
        {
        log.WriteLine("Game closed", LogType.INFO);
            return steam_instance.IsGameSupport(gameID);
        }

        //from FrozaHorizon.cs
        public bool StartPlayback(Dictionary<string, int> settings)
        {
        log.WriteLine("Game closed", LogType.INFO);
            return SetGameSettingsAndStartGamePlay(settings);
        }

        private bool SetGameSettingsAndStartGamePlay(Dictionary<string, int> allSettings)
        {
            bool isPageFound;
            bool isSettingsRequired = allSettings.Count > 0;
            isPageFound = CheckPage(OverWatchPage.StartPage, 120);
            if (isPageFound)
            {
            log.WriteLine("Game closed", LogType.INFO);
                log.WriteLine("Start page Found. Pressing Enter");
                Win32.SimulateKeyPressBySendInput(Keys.Enter);
                Thread.Sleep(3000);
            }
            else
            {
                log.WriteLine("Start Page not found");
                return false;
            }

            isPageFound = CheckPage(OverWatchPage.ContinueSettingsPage, 30);
            if (isPageFound)
            {
            log.WriteLine("Game closed", LogType.INFO);
                log.WriteLine("Continue settings page Found. Pressing Down and Enter");
                Win32.SimulateKeyPressBySendInput(Keys.Down);
                Thread.Sleep(500);
                Win32.SimulateKeyPressBySendInput(Keys.Enter);
                Thread.Sleep(3000);
            }

            isPageFound = CheckPage(OverWatchPage.OptionsPage, 120);
            if (isPageFound)
            {
            log.WriteLine("Game closed", LogType.INFO);
                log.WriteLine("Options page Found.");
                if (isSettingsRequired)
                {
                    log.WriteLine("Pressing Down for options ");
                    Win32.SimulateKeyPressBySendInput(Keys.Down);
                    Thread.Sleep(500);
                }
                log.WriteLine("Pressing Enter");
                Win32.SimulateKeyPressBySendInput(Keys.Enter);
                Thread.Sleep(3000);
            }
            else
            {
            log.WriteLine("Video Settings page Found. Pressing Enter to continue");
                    Win32.SimulateKeyPressBySendInput(Keys.Down);
                    Thread.Sleep(500);
                log.WriteLine("Options Page not found");
                return false;
            }
            if (isSettingsRequired)
            {
                isPageFound = CheckPage(OverWatchPage.VideoSettingsPage, 120);
                if (isPageFound)
                {
                    log.WriteLine("Video Settings page Found. Pressing Enter to continue");
                    Win32.SimulateKeyPressBySendInput(Keys.Down);
                    Thread.Sleep(500);
                    Win32.SimulateKeyPressBySendInput(Keys.Enter);
                    Thread.Sleep(500);
                }
                else
                {
                    log.WriteLine("Video settings Page not found");
                    return false;
                }

                isPageFound = CheckPage(OverWatchPage.BasicSettingsPage, 120);
                if (isPageFound)
                {
                    log.WriteLine("Basic Settings page Found. Clicking on Advance settings");
                    Rectangle rect = GetCoordinates("dynamic optimization", OverWatchPage.BasicSettingsPage);
                    if (rect.Width == 0)
                    {
                        log.WriteLine("Unable to find dynamic optimization button");
                        //return false;
                    }
                    else
                    {
                    log.WriteLine("Video Settings page Found. Pressing Enter to continue");
                    Win32.SimulateKeyPressBySendInput(Keys.Down);
                    Thread.Sleep(500);
                        ClickMidPoint(rect);
                        Win32.SimulateKeyPressBySendInput(Keys.Right);
                        Thread.Sleep(1000);
                    }
                    rect = GetCoordinates("advanced", OverWatchPage.BasicSettingsPage);
                    if (rect.Width == 0)
                    {
                        log.WriteLine("Unable to find advanced button");
                        return false;
                    }
                    ClickMidPoint(rect);
                }
                else
                {
                    log.WriteLine("Basic settings Page not found");
                    return false;
                }

                isPageFound = CheckPage(OverWatchPage.AdvanceSettingsPage, 10);
                if (!isPageFound)
                {
                    Win32.SimulateKeyPressBySendInput(Keys.Enter);
                    Thread.Sleep(1000);
                    isPageFound = CheckPage(OverWatchPage.AdvanceSettingsPage, 10);
                }

                if (isPageFound)
                {
                    log.WriteLine("Advanced Settings page Found");
                    string settingsName;
                    int valueCount;
                    foreach (KeyValuePair<string, int> setting in allSettings)
                    {
                        settingsName = setting.Key;
                        valueCount = setting.Value;
                        Rectangle rect = GetCoordinates(settingsName, OverWatchPage.AdvanceSettingsPage);
                        if (rect.Width == 0)
                        {
                            log.WriteLine("Unable to find " + settingsName);
                            return false;
                        }
                        else
                            log.WriteLine("Changinf setting " + settingsName + " to " + valueCount.ToString());
                        ClickMidPoint(rect);
                        ChangeSettingsValue(valueCount);
                    }
                    SaveSettings();
                    ExitSettings();
                }
                else
                {
                    log.WriteLine("Advanced settings Page not found");
                    return false;
                }
            }

            return StartGamePlayFromOptionsPage();
        }

        private bool CheckPage(OverWatchPage page, int timeoutInSec = 30)
        {
            string imageFile = Path.Combine(AtpEnvironment.ImagesTaskResultDir, page.ToString() + ".png");
            string dataFile;
            DateTime endTime = DateTime.Now.AddSeconds(timeoutInSec);
            bool isPageFound = false;
            while (DateTime.Now < endTime)
            {
                dataFile = GetCurrentPage(imageFile);
                if (IsRequiedPage(page, dataFile))
                {
                    isPageFound = true;
                    break;
                }
                Thread.Sleep(5000);
            }

            return isPageFound;
        }

        private bool IsRequiedPage(OverWatchPage page, string dataFileName = "CurrentScreen_data.json")
        {
            string uniqueWord = "";
            switch (page)
            {
                case OverWatchPage.StartPage:
                    uniqueWord = "start game";
                    break;
                case OverWatchPage.ContinueSettingsPage:
                    uniqueWord = "continue with existing settings";
                    break;
                case OverWatchPage.OptionsPage:
                    uniqueWord = "options";
                    break;
                case OverWatchPage.AdvanceSettingsPage:
                    uniqueWord = "shadow quality";
                    break;
                case OverWatchPage.BasicSettingsPage:
                    uniqueWord = "dynamic optimization";
                    break;
                case OverWatchPage.GameSettingsPage:
                    uniqueWord = @"difficulty settings\nadjust game difficulty";
                    break;
                case OverWatchPage.VideoSettingsPage:
                    uniqueWord = "benchmark mode";
                    break;
                case OverWatchPage.GamePausedPage:
                    uniqueWord = "reset car position";
                    break;
                case OverWatchPage.LoadingPage:
                    uniqueWord = "wait";
                    break;
                case OverWatchPage.GameStartPage:
                    uniqueWord = "main stage";
                    break;
            }

            Rectangle rect = GetCoordinates(uniqueWord, dataFileName);
            return rect.Width != 0;
        }

        private Rectangle GetCoordinates(string word, string resultFile = "data.json")
        {
            Rectangle rect = new Rectangle();
            if (File.Exists(resultFile))
            {
                string dataFromFile = File.ReadAllText(resultFile);
                if (!string.IsNullOrEmpty(dataFromFile))
                {
                    var data = (JArray)JsonConvert.DeserializeObject(dataFromFile);
                    if (data[0].ToString().ToUpper() == "TRUE")
                    {
                        foreach (var dataentry in data[1]["result"][0]["boxes"])
                        {
                            foreach (string str in dataentry["Words"])
                            {
                                if (str.Contains(word))
                                {
                                    Console.WriteLine(dataentry["Coordinates"]);
                                    rect.X = int.Parse(dataentry["Coordinates"][0].ToString());
                                    rect.Y = int.Parse(dataentry["Coordinates"][1].ToString());
                                    rect.Width = int.Parse(dataentry["Coordinates"][2].ToString());
                                    rect.Height = int.Parse(dataentry["Coordinates"][3].ToString());
                                }
                            }
                        }
                    }
                    else
                    {
                        log.WriteLine("Seems request not completed");
                    }
                }
                else
                {
                    log.WriteLine("No data found in file " + resultFile);
                }
            }
            else
                log.WriteLine(resultFile + " not found");

            return rect;
        }






    }
}
