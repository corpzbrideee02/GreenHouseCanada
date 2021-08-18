/*
 * Program:         GreenHouseGasesCanada.exe
 * Authors:         Dianne Corpuz (section 2) + Brittany Diesbourg (section 4)
 * Course:          INFO-3138
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Xml;           
using System.Xml.XPath;     // XPathNavigator class

namespace GreenHouseGasesCanada
{
    class Program
    {  // Constants
        private const string _XML_FILE = "ghg-canada.xml";  // File is in the bin\Debug\netcoreapp3.1 folder
        public static Dictionary<string, int> regionMap, sourceMap;
        public static int startingYearChoice=2015, endingYearChoice=2019, yearLength = endingYearChoice - startingYearChoice;
        public const int LAST_POSSIBLE_YEAR = 2019;

        static void Main(string[] args)
        {
            PrintProgramTitle();
            char menuChoice;
            try
            {
                XmlDocument doc = new XmlDocument();  // Load XML file into the DOM
                doc.Load(_XML_FILE);
                LoadXML(doc);
                XPathNavigator nav = doc.CreateNavigator(); // Create an XPathNavigator object for performing XPath queries

                do
                {
                    menuChoice = ProcessMenu();   // Get user selection
                    Console.WriteLine();

                    switch (menuChoice){
                        case 'Y':
                            AdjustRangeOfYears(nav);
                            break;
                        case 'R':
                            SelectRegion(nav);
                            break;
                        case 'S':
                            SelectGHGSource(nav);
                            break;
                        case 'X':
                            Console.WriteLine("Exit Program");
                            break;
                    }
                } while (!menuChoice.Equals('X'));
            }
            catch (XmlException err){
                Console.WriteLine("\nXML ERROR: " + err.Message);
            }
            catch (Exception err){
                Console.WriteLine("\nERROR: " + err.Message);
            }
        }//end of Main

        private static void PrintProgramTitle()
        {
            Console.WriteLine("Greenhouse Gas Emissions in Canada");
            Console.WriteLine("==================================");
        }

        private static char ProcessMenu()
        {
            Console.Write("\n+------------------------------------------------+\n"
                          + "|  'Y' to adjust the range of years               |\n"
                          + "|  'R' to select a region                         |\n"
                          + "|  'S' to select a specific GHG source            |\n"
                          + "|  'X' to exit the program                        |\n"
                          + "+------------------------------------------------+\n");

            char userInput;
            bool validInput;
            char[] validCharacter = { 'Y', 'R', 'S', 'X' };
            do
            {
                Console.Write("Your selection: ");
                userInput = Char.ToUpper(Console.ReadKey().KeyChar);
                validInput = (new string(validCharacter).Contains(userInput)) ? true : false;
                if (validInput == false) {
                    Console.WriteLine("\nERROR: You must key-in a valid character from the menu.\n");
                }

            } while (!validInput);

            return userInput;
        } // end proccessMenu()


        private static void AdjustRangeOfYears(XPathNavigator nav)
        {
            ValidYear("Starting year (1990 to 2019): ", "startYear");
            ValidYear("Ending year (1990 to 2019): ", "endYear");
            Console.WriteLine("Press any key to continue.");
            Console.ReadLine();
        }

        private static void ValidYear(string promptMessage, string type)
        {
            bool valid_year = false;
            string data;

            if (startingYearChoice == LAST_POSSIBLE_YEAR && type.Equals("endYear")) // skips ending year input if starting year is 2019 as it would just be the same
            {
                Console.WriteLine("Ending year (1990 to 2019): 2019");
                return;
            }

            do
            {
                Console.Write($"{promptMessage}");
                data = Console.ReadLine();
                valid_year = int.TryParse(data, out int temp);

                if (type.Equals("startYear"))
                {
                    if (valid_year)
                    {
                        valid_year = temp >= 1990 && temp <= LAST_POSSIBLE_YEAR;

                        if (valid_year)
                            startingYearChoice = temp;
                        else
                            Console.WriteLine($"\tERROR: Starting year must be an integer between 1990 and 2019");                 
                    }                       
                    else
                        Console.WriteLine($"\tERROR: Starting year must be a number");
                }
                else if (type.Equals("endYear"))
                {
                  yearLength = LAST_POSSIBLE_YEAR - startingYearChoice;

                    if (valid_year)
                    {                      
                        if (startingYearChoice > 2015)
                        {
                            valid_year = temp <= LAST_POSSIBLE_YEAR && temp >= startingYearChoice; // prevents user from picking a year after 2019 and before starting year                            
                            if (!valid_year)
                                Console.WriteLine($"\tERROR: Ending year must be an integer between {startingYearChoice} and {startingYearChoice + yearLength}");
                        }                            
                        else
                        {
                            valid_year = temp < (startingYearChoice + 5) && temp >= startingYearChoice; // prevents user from picking a year more than 4 years after starting year and any years before
                            if (!valid_year)
                                Console.WriteLine($"\tERROR: Ending year must be an integer between {startingYearChoice} and {startingYearChoice + 4}");
                        }
                                                                                                 
                        if (valid_year)
                            endingYearChoice = temp;
                    }
                    else
                        Console.WriteLine($"\tERROR: Ending year must be a number");
                }
            } while (!valid_year);
        }

        private static void LoadXML(XmlDocument doc)
        {
            //to store key-value pair
            regionMap = new Dictionary<string, int>();
            sourceMap = new Dictionary<string, int>();

            //declaration
            String regionName, sourceDescription;
            XmlAttributeCollection regionAttrs, sourceAttrs;
            XmlNodeList allRegion = doc.GetElementsByTagName("region");
            XmlNodeList allSource;

            // Loop through all the "region" elements
            for (int i = 0; i < allRegion.Count; i++)
            {
                regionAttrs = allRegion.Item(i).Attributes;  // Get all attributes for this region element
                regionName = regionAttrs.GetNamedItem("name").InnerText;  // Get the values of region attributes

                allSource = allRegion.Item(i).ChildNodes; // Get all the child nodes (elements in this case) of this region element

                for (int j = 0; j < allSource.Count; j++) {
                   
                    sourceAttrs = allSource.Item(j).Attributes;  // Get all attributes for this region element
                    sourceDescription = sourceAttrs.GetNamedItem("description").InnerText;
                    if (!sourceMap.ContainsKey(sourceDescription))
                        sourceMap.Add(sourceDescription, j + 1);
                }
             regionMap.Add(regionName, i + 1); //add to the regionName map
            }
        }

        private static void SelectRegion(XPathNavigator nav)
        {
            Console.WriteLine("Select a region by number as shown below...");
            foreach (KeyValuePair<string, int> keyValue in regionMap){
                Console.WriteLine("{0}. {1}", keyValue.Value, keyValue.Key);
            }

            bool valid_number;
            do
            {
                Console.Write("\nEnter a region #: ");
                valid_number = int.TryParse(Console.ReadLine(), out int regionNumber);

                if (valid_number && regionMap.ContainsValue(regionNumber))
                {
                    string myKey = regionMap.FirstOrDefault(x => x.Value == regionNumber).Key;
                    PrintTitle($"in {myKey}");

                    XPathNodeIterator dataIt, sourceIt; //data and source Iterator
                    List<string> listOfEmission = new List<string>();
                    List<string> listOfDescription = new List<string>();

                    Console.Write("{0,55}\t", "Source");
                    for (int i = startingYearChoice; i <= endingYearChoice; i++)
                    {
                        Console.Write(" {0,9}", i);
                        sourceIt = nav.Select($"//region[@name='{myKey}']/source/@description");

                        while (sourceIt.MoveNext())
                        {
                            dataIt = nav.Select($"//region[@name='{myKey}']/source[@description='{sourceIt.Current.Value}']/emissions[@year='{i}']");

                            if (dataIt.MoveNext())
                                listOfEmission.Add(dataIt.Current.Value);
                            else
                                listOfEmission.Add("-");

                            if (!listOfDescription.Contains(sourceIt.Current.Value))
                                listOfDescription.Add(sourceIt.Current.Value);
                        }
                    }//END OF FOR LOOP 

                    Console.WriteLine();
                    yearLength = endingYearChoice - startingYearChoice;
                    string[,] finalDataFormat = new string[yearLength + 1, sourceMap.Count()];

                    for (int y = 0; y < listOfDescription.Count; ++y)
                    {
                        int k = y, m = 0;
                        Console.Write("{0,55}\t", listOfDescription[y].ToString());
                        for (int x = 0; x <= yearLength; x++)
                        {
                            int offset = k + (listOfDescription.Count * m++);
                            valid_number = double.TryParse(listOfEmission[offset], out double temp);

                            if (valid_number)

                                finalDataFormat[x, y] = string.Format("{0:0.000}", temp);
                            else
                                finalDataFormat[x, y] = "-";
                            Console.Write(" {0,9}", finalDataFormat[x, y]);
                        }
                        Console.WriteLine();
                        valid_number = true;
                    }
                }
                else
                {
                    valid_number = false;
                    Console.WriteLine($"\tERROR: Input must be a number from the list");
                }
                    
            } while (!valid_number);

        }

        private static void PrintTitle(string _title)
        {
            Console.WriteLine($"\n\nEmissions {_title} (Megatonnes)");
            Console.WriteLine("------------------------------------------");
        }

        private static void  SelectGHGSource(XPathNavigator nav)
        {
            Console.WriteLine("Select a source by number as shown below...");
            foreach (KeyValuePair<string, int> keyValue in sourceMap) {
                Console.WriteLine("{0}. {1}", keyValue.Value, keyValue.Key);
            }
            
            bool valid_number;
            do
            {
                Console.Write("\nEnter a source #: ");
                valid_number = int.TryParse((Console.ReadKey().KeyChar).ToString(), out int sourceNumber);

                if (valid_number && sourceMap.ContainsValue(sourceNumber))
                {
                    string myKey = sourceMap.FirstOrDefault(x => x.Value == sourceNumber).Key;
                    PrintTitle($"from {myKey}");

                    XPathNodeIterator dataIt, regionIt;//emission and region Iterator
                    List<string> listOfEmission = new List<string>();
                    List<string> listOfRegion = new List<string>();

                    Console.Write("{0,40}\t", "Region");
                    for (int i = startingYearChoice; i <= endingYearChoice; i++)
                    {
                        Console.Write(" {0,9}", i);
                        regionIt = nav.Select($"//region/@name");
                        while (regionIt.MoveNext())
                        {
                            dataIt = nav.Select($"//region[@name='{regionIt.Current.Value}']/source[@description='{myKey}']/emissions[@year='{i}']");

                            if (dataIt.MoveNext())
                                listOfEmission.Add(dataIt.Current.Value);
                            else
                                listOfEmission.Add("-");

                            if (!listOfRegion.Contains(regionIt.Current.Value))
                                listOfRegion.Add(regionIt.Current.Value);

                        }//end of while RegionIt

                    }//END OF FOR LOOP 

                    yearLength = endingYearChoice - startingYearChoice;
                    Console.WriteLine();
                    string[,] finalDataFormat = new string[yearLength + 1, regionMap.Count()];

                    for (int y = 0; y < listOfRegion.Count; ++y)
                    {
                        int k = y, m = 0;
                        Console.Write("{0,40}\t", listOfRegion[y].ToString());
                        for (int x = 0; x <= yearLength; x++)
                        {
                            int offset = k + (listOfRegion.Count * m++);
                            valid_number = double.TryParse(listOfEmission[offset], out double temp);

                            if (valid_number)

                                finalDataFormat[x, y] = string.Format("{0:0.000}", temp);
                            else
                                finalDataFormat[x, y] = "-";

                            Console.Write(" {0,9}", finalDataFormat[x, y]);
                        }
                        Console.WriteLine();
                        valid_number = true;
                    }
                }
                else
                {
                    valid_number = false;
                    Console.WriteLine($"\tERROR: Input must be a number from the list");
                }
            } while (!valid_number);
        }//end of void SelectGHG Source
    }
}
