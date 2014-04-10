﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;
using MetroFramework.Forms;

namespace MistClient
{
    class Util
    {
        public static void LoadTheme(MetroFramework.Components.MetroStyleManager MetroStyleManager)
        {
            Friends.globalThemeManager.Add(MetroStyleManager);
            MetroStyleManager.Theme = Friends.globalStyleManager.Theme;
            MetroStyleManager.Style = Friends.globalStyleManager.Style;
        }

        public static string HTTPRequest(string url)
        {
            var result = "";
            try
            {
                using (var webClient = new WebClient())
                {
                    using (var stream = webClient.OpenRead(url))
                    {
                        using (var streamReader = new StreamReader(stream))
                        {
                            result = streamReader.ReadToEnd();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var wtf = ex.Message;
            }

            return result;
        }

        public static HttpWebResponse Fetch(string url)
        {
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "POST";
            HttpWebResponse response;
            for (int count = 0; count < 10; count++)
            {
                try
                {
                    response = request.GetResponse() as HttpWebResponse;
                    return response;
                }
                catch
                {
                    System.Threading.Thread.Sleep(100);
                    Console.WriteLine("retry");
                }
            }
            return null;
        }

        public static string ParseBetween(string Subject, string Start, string End)
        {
            return Regex.Match(Subject, Regex.Replace(Start, @"[][{}()*+?.\\^$|]", @"\$0") + @"\s*(((?!" + Regex.Replace(Start, @"[][{}()*+?.\\^$|]", @"\$0") + @"|" + Regex.Replace(End, @"[][{}()*+?.\\^$|]", @"\$0") + @").)+)\s*" + Regex.Replace(End, @"[][{}()*+?.\\^$|]", @"\$0"), RegexOptions.IgnoreCase).Value.Replace(Start, "").Replace(End, "");
        }

        public static string[] GetStringInBetween(string strBegin,
                                                  string strEnd, string strSource,
                                                  bool includeBegin, bool includeEnd)
        {
            string[] result = { "", "" };
            int iIndexOfBegin = strSource.IndexOf(strBegin);
            if (iIndexOfBegin != -1)
            {
                // include the Begin string if desired
                if (includeBegin)
                    iIndexOfBegin -= strBegin.Length;
                strSource = strSource.Substring(iIndexOfBegin
                    + strBegin.Length);
                int iEnd = strSource.IndexOf(strEnd);
                if (iEnd != -1)
                {
                    // include the End string if desired
                    if (includeEnd)
                        iEnd += strEnd.Length;
                    result[0] = strSource.Substring(0, iEnd);
                    // advance beyond this segment
                    if (iEnd + strEnd.Length < strSource.Length)
                        result[1] = strSource.Substring(iEnd
                            + strEnd.Length);
                }
            }
            else
                // stay where we are
                result[1] = strSource;
            return result;
        }

        public static string GetPrice(int defindex, int quality, SteamTrade.Inventory.Item inventoryItem, bool gifted = false, int attribute = 0)
        {
            try
            {
                var item = SteamTrade.Trade.CurrentSchema.GetItem(defindex);
                string craftable = inventoryItem.IsNotCraftable ? "Non-Craftable" : "Craftable";
                double value = BackpackTF.CurrentSchema.Response.Items[item.ItemName].Prices[quality.ToString()]["Tradable"][craftable]["0"].Value;
                double keyValue = BackpackTF.KeyPrice;
                double billsValue = BackpackTF.BillPrice * keyValue;
                double budValue = BackpackTF.BudPrice * keyValue;

                string result = "";

                if (inventoryItem.IsNotCraftable)
                {
                    value = value / 2.0;
                }
                if (inventoryItem.IsNotTradeable)
                {
                    value = value / 2.0;
                }
                if (gifted)
                {
                    value = value * 0.75;
                }
                if (quality == 3)
                {
                    if (item.CraftMaterialType == "weapon")
                    {
                        int level = inventoryItem.Level;
                        switch (level)
                        {
                            case 0:
                                value = billsValue + 5.11;
                                break;
                            case 1:
                                value = billsValue;
                                break;
                            case 42:
                                value = value * 10.0;
                                break;
                            case 69:
                                value = billsValue;
                                break;
                            case 99:
                                value = billsValue;
                                break;
                            case 100:
                                value = billsValue;
                                break;
                            default:
                                break;
                        }
                    }
                    else if (item.CraftMaterialType == "hat")
                    {
                        int level = inventoryItem.Level;
                        switch (level)
                        {
                            case 0:
                                value = value * 10.0;
                                break;
                            case 1:
                                value = value * 5.0;
                                break;
                            case 42:
                                value = value * 3.0;
                                break;
                            case 69:
                                value = value * 4.0;
                                break;
                            case 99:
                                value = value * 4.0;
                                break;
                            case 100:
                                value = value * 6.0;
                                break;
                            default:
                                break;
                        }
                    }
                }

                if (value >= budValue * 1.33)
                {
                    value = value / budValue;
                    result = value.ToString("0.00") + " buds";
                }
                else if (value >= keyValue && !item.ItemName.Contains("Crate Key"))
                {
                    value = value / keyValue;
                    result = value.ToString("0.00") + " keys";
                }
                else
                {
                    result = value.ToString("0.00") + " ref";
                }

                return result;
            }
            catch
            {
                return "Unknown";
            }
        }
    }
}
