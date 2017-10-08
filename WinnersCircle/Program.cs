/*************************
 * Author: Rants of Developer - Realname hidden
 * Public Email rantofdev@gmail.com
 * About: Uses a CSV to facilitate a game giveaway. Output the result to files for the user to use the Online randomizer or built in randomizer.
 * Permision: MIT - Free to use anywhere just credit rantofdev@gmail.com and include the licence (in the github repo).  
 *************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace WinnersCircle
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Make Sure file name is named FormResponses in the directory where this program is run.\nPress Any key once this file is there.");
            Console.ReadKey();

            // Input file.
            string pathToInputFile = @"FormResponses.csv";
            // Per game the list of users.
            Dictionary<string, List<string>> dictionary = new Dictionary<string, List<string>>();

            // Read the CSV
            string[] dataResponse = File.ReadAllLines(pathToInputFile, Encoding.UTF8);

            // index = 1 to skip the headers of the csv
            for (int index = 1; index < dataResponse.Length; index++)
            {
                // Seperating the game name and users. CSV uses timestamp, "Game List, ...", User
                int firstComma = dataResponse[index].IndexOf(",");
                int lastComma = dataResponse[index].LastIndexOf(",");

                // get the gamelist in the middel and remove the '"', (lastComma- firstComma) is the legnth from the first to the last.
                string gameList = dataResponse[index].Substring(firstComma + 1, (lastComma - firstComma - 1)).Replace("\"", "");

                // Get the user after the last comma, Remove any special char from the users names
                string user = Regex.Replace(dataResponse[index].Substring(lastComma + 1), @"[^0-9a-zA-Z]+", "");

                // If no game pressent dont add to the dictionary
                if (!string.IsNullOrWhiteSpace(gameList))
                {
                    // Get the game list
                    string[] gameSplit = gameList.Split(',');

                    // foreach game in the gamelist
                    foreach (string game in gameSplit)
                    {
                        // Remove the pre space
                        string gameName = (game[0] == ' ' ? game.Substring(1) : game);

                        // See if the game falready exists in the dictionary.
                        if (!dictionary.Keys.Contains(gameName))
                        {
                            // initialise the game dictionary with the list of users
                            dictionary.Add(gameName, new List<string>());
                            // Add the user to the game
                            dictionary[gameName].Add(user);
                        }
                        else
                        {
                            // Check to see if the user has entered the comp already.
                            if (!dictionary[gameName].Contains(user.ToLower()))
                            {
                                // Set the usernames to lower so we can use a quick and easy (Dirty) way to check for duplicates
                                dictionary[gameName].Add(user.ToLower());
                            }
                        }
                    }
                }
            }
            foreach (string gameNames in dictionary.Keys)
            {
                // Logging to see the outcome of the files
                Console.WriteLine("Creating file: " + Regex.Replace(gameNames, @"[^0-9a-zA-Z]+", "_") + @".txt");
                // Write the game as the file and the people that want it in the file
                System.IO.File.WriteAllLines(Regex.Replace(gameNames, @"[^0-9a-zA-Z]+", "_") + @".txt", dictionary[gameNames]);
            }
            Console.WriteLine("======================================================================");
            foreach (string gameNames in dictionary.Keys)
            {
                System.Threading.Thread.Sleep((int)(new Random().NextDouble() * 1000));
                Random rand = new Random(DateTime.Now.Millisecond * DateTime.Now.Second * (int)(Math.PI * 100));
                int winner = rand.Next(dictionary[gameNames].Count);
                StreamWriter stream = File.AppendText("Winners.txt");
                Console.WriteLine("Name: {0,40} - Game: {1, 40}.", dictionary[gameNames][winner], gameNames);
                stream.WriteLine("Name: {0,40} - Game: {1, 40}.", dictionary[gameNames][winner], gameNames);
                stream.Close();
            }
            Console.WriteLine("======================================================================");
            Console.ReadKey();
        }
    }
}