﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Text.RegularExpressions;/*
using MicrosoftResearch.Infer;
using MicrosoftResearch.Infer.Collections;
using MicrosoftResearch.Infer.Distributions;
using MicrosoftResearch.Infer.Factors;
using MicrosoftResearch.Infer.Graphs;
using MicrosoftResearch.Infer.Models;
using MicrosoftResearch.Infer.Maths;
using MicrosoftResearch.Infer.Transforms;
using MicrosoftResearch.Infer.Utils;
using MicrosoftResearch.Infer.Views;
*/
using ZeldaInfer.LevelParse;
namespace ZeldaInfer {
	class Program {
		
		static void ModelNetworkSprinklerFile() {

			GraphicalModel model = new GraphicalModel("WetRainSprinkler.xml");
			model.CreateNetwork();
			Dictionary<string, int[]> observedData = GraphicalModel.LoadData("WetRainSprinklerData.xml");
			model.LearnParameters(observedData);
		}
		 
		static void RunAllLevels() {
			string[] levels = new string[]{
                //"Levels/LA 1.xml","Levels/LA 2.xml","Levels/LA 3.xml","Levels/LA 4.xml",
                //"Levels/LA 6.xml","Levels/LA 7.xml",
                //"Levels/LA 8.xml","Levels/LoZ 1.xml",
                //"Levels/LoZ 2.xml","Levels/LoZ 3.xml","Levels/LoZ 4.xml","Levels/LoZ 5.xml",
                //"Levels/LoZ 7.xml","Levels/LoZ 8.xml","Levels/LoZ 9.xml","Levels/LoZ2 1.xml",
                //"Levels/LoZ2 2.xml","Levels/LoZ2 4.xml","Levels/LoZ2 5.xml","Levels/LoZ2 6.xml",
                //"Levels/LoZ2 7.xml","Levels/LoZ2 8.xml",
                //"Levels/LoZ2 9.xml",
                "Levels/LttP 1.xml",
                //"Levels/LttP 10.xml",
                //"Levels/LttP 11.xml",
                //"Levels/LttP 2.xml","Levels/LttP 3.xml",
                //"Levels/LttP 4.xml","Levels/LttP 5.xml","Levels/LttP 6.xml","Levels/LttP 7.xml",
                //"Levels/LttP 8.xml","Levels/LttP 9.xml",
			};

			foreach (var level in levels) {
				Console.WriteLine(level);
				Dungeon dungeon = new Dungeon(level);
				SearchAgent path = dungeon.getOptimalPath(level.Contains("LttP"));
				Console.WriteLine(path.pathToString());
				dungeon.UpdateRooms(path);
				string output = level;
				output = Regex.Replace(output, @"Levels", "Summaries");
				output = Regex.Replace(output, " ", "");
				dungeon.WriteStats(output, path);
			}
		}
		static void CreateGraphicalModelFiles() {
            string[] summaries = new string[]{
                "Summaries/LA1.xml","Summaries/LA2.xml","Summaries/LA3.xml",
                "Summaries/LA4.xml","Summaries/LA6.xml","Summaries/LA7.xml",
                "Summaries/LA8.xml","Summaries/LoZ1.xml","Summaries/LoZ2.xml",
                "Summaries/LoZ21.xml","Summaries/LoZ22.xml","Summaries/LoZ24.xml",
                "Summaries/LoZ25.xml","Summaries/LoZ26.xml","Summaries/LoZ27.xml",
                "Summaries/LoZ28.xml","Summaries/LoZ29.xml","Summaries/LoZ3.xml",
                "Summaries/LoZ4.xml","Summaries/LoZ5.xml","Summaries/LoZ7.xml",
                "Summaries/LoZ8.xml","Summaries/LoZ9.xml","Summaries/LttP1.xml",
                "Summaries/LttP10.xml","Summaries/LttP11.xml","Summaries/LttP2.xml",
                "Summaries/LttP3.xml","Summaries/LttP4.xml","Summaries/LttP5.xml",
                "Summaries/LttP6.xml","Summaries/LttP7.xml","Summaries/LttP8.xml",
                "Summaries/LttP9.xml",
            };
            HashSet<string> wholeLevelParameters = new HashSet<string>(){
                "roomsInLevel",  "enemyRoomsInLevel",  "puzzleRoomsInLevel",
                "itemRoomsInLevel",  "doorsInLevel",  "passableDoorsInLevel",
                "lockedDoorsInLevel", "bombLockedDoorsInLevel",  "bigKeyDoorsInLevel",
                "oneWayDoorsInLevel",  "itemLockedDoorsInLevel",  "puzzleLockedDoorsInLevel",
                "softLockedDoorsInLevel",  "lookAheadsInLevel",  "totalCrossings",
                "maximumCrossings",  "maximumDistanceToPath",  "pathLength",
                "roomsOnPath",  "enemyRoomsOnPath",  "puzzleRoomsOnPath",
                "itemRoomsOnPath",  "doorsOnPath",  "lockedDoorsOnPath",
                "bombLockedDoorsOnPath",  "bigKeyLockedDoorsOnPath",  "itemLockedDoorsOnPath",
                "softLockedDoorsOnPath",  "puzzleLockedDoorsOnPath",  "lookAheadsOnPath",
                "oneWayDoorsOnPath",  "distanceToDungeonKey",  "distanceToSpecialItem",
            };
            Dictionary<string, string> summaryDictionary = new Dictionary<string, string>();
            int totalCount = 0;
            foreach (var summary in summaries) {
                XDocument summaryDoc = XDocument.Load(summary);
                Dictionary<string, string> levelParams = new Dictionary<string, string>();
                Dictionary<string, string> roomParams = new Dictionary<string, string>();
                int copies = 0;
                foreach (XElement element in summaryDoc.Root.Descendants()) {
                    if (wholeLevelParameters.Contains(element.Name.LocalName)) {
                        levelParams[element.Name.LocalName] = element.Value;
                    }
                    else {
                        roomParams[element.Name.LocalName] = element.Value;
                        copies = element.Value.Count(f => f == ';')+1;
                    }
                    /*
                    if (summaryDictionary.ContainsKey(element.Name.LocalName)) {

                    }
                    else {
                        summaryDictionary[element.Name.LocalName] = element.Value + "|";
                    }
                     * */
                }
                totalCount += copies;
                foreach (var pair in levelParams) {
                    if (summaryDictionary.ContainsKey(pair.Key)) {
                        summaryDictionary[pair.Key] += string.Concat(Enumerable.Repeat(pair.Value + ";", copies));
                    }
                    else {
                        summaryDictionary[pair.Key] = string.Concat(Enumerable.Repeat(pair.Value + ";", copies));
                    }
                }
                foreach (var pair in roomParams) {
                    if (summaryDictionary.ContainsKey(pair.Key)) {
                        summaryDictionary[pair.Key] += pair.Value + ";";
                    }
                    else {
                        summaryDictionary[pair.Key] = pair.Value + ";";
                    }
                }
            }
            XDocument categoriesDoc = new XDocument(new XElement("root"));
            Console.WriteLine(totalCount);
            Dictionary<string, List<string>> categories = new Dictionary<string, List<string>>();
            foreach (var category in summaryDictionary) {
                categories[category.Key] = new List<string>(new SortedSet<string>(category.Value.Split(';')));
                categories[category.Key].Remove("");
                categoriesDoc.Root.Add(new XElement(category.Key, new XAttribute("count", categories[category.Key].Count), string.Join(";", categories[category.Key].ToArray())));
           
            //    Console.WriteLine(str);
            }
            categoriesDoc.Save("categories.xml");
			XDocument xdoc = XDocument.Load("BayesNetwork.xml");
            Dictionary<string, string> nodes = new Dictionary<string, string>();
            List<Tuple<string, string>> edges = new List<Tuple<string, string>>();
			foreach (XElement element in xdoc.Root.Descendants()) {
				string val = "";
				if (element.Attribute("value") != null) {
					val = element.Attribute("value").Value.Split('&')[0];
					val = Regex.Replace(val, " ", ",");
				}
                if (element.Attribute("style") != null) {
                    string style = element.Attribute("style").Value;
                    if (style.Contains("ellipse")) {
                        nodes[element.Attribute("id").Value] = val;
                    }
                    else if (style.Contains("edgeStyle")) {
                        edges.Add(new Tuple<string, string>(element.Attribute("source").Value, element.Attribute("target").Value));
                    }
                }
			}
            XDocument dungeonDoc = new XDocument(new XElement("root"));
            foreach (var category in categories) {

                dungeonDoc.Root.Add(new XElement("Category", new XAttribute("name", category.Key + "Category"), new XAttribute("categories", string.Join(",", 
                    Enumerable.Range(0, category.Value.Count)))));
            }
            foreach (var node in nodes.Values) {
                dungeonDoc.Root.Add(new XElement("Node", new XAttribute("name", node), new XAttribute("category", node + "Category")));
            }
            foreach (var edge in edges) {
                dungeonDoc.Root.Add(new XElement("Edge", new XAttribute("parent", nodes[edge.Item1]), new XAttribute("child", nodes[edge.Item2])));
            }
            dungeonDoc.Save("dungeonNetwork.xml");
            XDocument dataDoc = new XDocument(new XElement("root"));
            foreach (var param in summaryDictionary) {
                int subtract = 1;
                
                dataDoc.Root.Add(new XElement("Data", new XAttribute("name", param.Key),string.Join(",",param.Value.Substring(0,param.Value.Length-1).Split(';').Select(p => categories[param.Key].IndexOf(p)))  ));
                
            }
            dataDoc.Save("dungeonNetworkData.xml");
		}
        static void CreateGraphicalModel() {

            GraphicalModel model = new GraphicalModel("dungeonNetwork.xml");
            model.CreateNetwork();
            Dictionary<string, int[]> observedData = GraphicalModel.LoadData("dungeonNetworkData.xml");
            model.LearnParameters(observedData);
        }

		static void Main(string[] args) {
           // RunAllLevels();
            CreateGraphicalModelFiles();
            CreateGraphicalModel();
        //    ModelNetworkSprinklerFile();
			Console.WriteLine("ALL DONE :)");
			Console.Read();
		}
	}
}
