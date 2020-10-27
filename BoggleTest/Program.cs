//Charlie Schroeder, October 2020
//Compiler Used: Visual Studio 2019 Version 16.6.2
//  This test file is confidential. No part of this document may be disclosed publicly 
//  without prior written consent from ZeniMax Media.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

public class BoardLetter
{
	public char value;
	public bool hasBeenUsed;
	public int position;
	/* positions:
	 * 0  1  2  3
	 * 4  5  6  7
	 * 8  9  10 11
	 * 12 13 14 15
	 */
}

class BoggleTest
{
	public static List<string> foundWords;
	public static string dictionaryPath;
	static void Main(string[] args)
	{
		string outputFileName;

		if (args.Length != 3)
		{
			Console.WriteLine("Usage: BoggleTest <dictionary_filename> <board_filename> <output_filename>");
		}

		dictionaryPath = args[0];
		List<BoardLetter> board = ParseBoardToList(args[1]);
		outputFileName = args[2];

		foundWords = new List<string>();

		SolveBoard(board);

		//alphabatize
		foundWords.Sort();

		File.WriteAllLines(outputFileName, foundWords);

		Console.WriteLine("Finished. Results output to " + outputFileName);
	}

	private static List<BoardLetter> ParseBoardToList(string fileName)
	{
		List<BoardLetter> board = new List<BoardLetter>();
		//this will read the letters in order from left to right, top to bottom
		char[] letters = File.ReadAllText(fileName).ToCharArray();

		int index = 0;

		foreach (var letter in letters)
		{
			if (!String.IsNullOrWhiteSpace(letter.ToString()))
			{
				BoardLetter boardLetter = new BoardLetter();
				boardLetter.value = letter;
				boardLetter.position = index;
				boardLetter.hasBeenUsed = false;

				board.Add(boardLetter);

				index++;
			}
		}
		return board;
	}

	public static void SolveBoard(List<BoardLetter> board)
	{
		foreach (var letter in board)
		{
			Traverse(letter.position, "", board);

			//reset the search
			board.ForEach(x => x.hasBeenUsed = false);
		}
	}

	public static void Traverse(int currentIndex, string currentPath, List<BoardLetter> board)
	{
		currentPath += board[currentIndex].value;

		board[currentIndex].hasBeenUsed = true;

		bool hasPossibleMatches = true;

		if (currentPath.Length >= 3)
		{
			hasPossibleMatches = CheckWord(currentPath);
		}

		//words can quickly become gibberish, so stop if there are no possible permutations
		if (hasPossibleMatches)
		{
			MoveNorth(currentIndex, currentPath, board);
			MoveSouth(currentIndex, currentPath, board);
			MoveEast(currentIndex, currentPath, board);
			MoveNorthEast(currentIndex, currentPath, board);
			MoveSouthEast(currentIndex, currentPath, board);
			MoveWest(currentIndex, currentPath, board);
			MoveNorthWest(currentIndex, currentPath, board);
			MoveSouthWest(currentIndex, currentPath, board);
		}

		//clear this here because lists are pass by reference
		board[currentIndex].hasBeenUsed = false;
	}

	public static void MoveNorth(int currentIndex, string currentPath, List<BoardLetter> board)
	{
		if (currentIndex > 3 && !board[currentIndex - 4].hasBeenUsed)
		{
			//move north
			Traverse(currentIndex - 4, currentPath, board);
		}
	}

	public static void MoveSouth(int currentIndex, string currentPath, List<BoardLetter> board)
	{
		if (currentIndex < 12 && !board[currentIndex + 4].hasBeenUsed)
		{
			//move south
			Traverse(currentIndex + 4, currentPath, board);

		}
	}

	public static void MoveEast(int currentIndex, string currentPath, List<BoardLetter> board)
	{
		if ((currentIndex + 1) % 4 > 0 && !board[currentIndex + 1].hasBeenUsed)
		{
			//move east
			Traverse(currentIndex + 1, currentPath, board);
		}
	}

	public static void MoveNorthEast(int currentIndex, string currentPath, List<BoardLetter> board)
	{
		if ((currentIndex + 1) % 4 > 0)
		{
			if (currentIndex > 3 && !board[currentIndex - 3].hasBeenUsed)
			{
				//move north east
				Traverse(currentIndex - 3, currentPath, board);
			}
		}
	}

	public static void MoveSouthEast(int currentIndex, string currentPath, List<BoardLetter> board)
	{
		if ((currentIndex + 1) % 4 > 0)
		{
			if (currentIndex < 11 && !board[currentIndex + 5].hasBeenUsed)
			{
				//move south east
				Traverse(currentIndex + 5, currentPath, board);
			}
		}
	}

	public static void MoveWest(int currentIndex, string currentPath, List<BoardLetter> board)
	{
		if (currentIndex % 4 > 0 && !board[currentIndex - 1].hasBeenUsed)
		{
			//move west
			Traverse(currentIndex - 1, currentPath, board);
		}
	}

	public static void MoveNorthWest(int currentIndex, string currentPath, List<BoardLetter> board)
	{
		if (currentIndex % 4 > 0)
		{
			if (currentIndex > 3 && !board[currentIndex - 5].hasBeenUsed)
			{
				//move northwest
				Traverse(currentIndex - 5, currentPath, board);
			}
		}
	}

	public static void MoveSouthWest(int currentIndex, string currentPath, List<BoardLetter> board)
	{
		if (currentIndex % 4 > 0)
		{
			if (currentIndex < 12 && !board[currentIndex + 3].hasBeenUsed)
			{
				//move southwest
				Traverse(currentIndex + 3, currentPath, board);
			}
		}
	}

	public static bool CheckWord(string currentPath)
	{
		string line;
		bool hasPossibleMatches = false;
		StreamReader stream = new StreamReader(dictionaryPath);

		//q's count as qu in this version of the game
		string replaceQ = Regex.Replace(currentPath.ToString(), "Q", "QU").ToLower();

		while ((line = stream.ReadLine()) != null && !hasPossibleMatches)
		{
			line = line.Trim().ToLower();

			if (replaceQ == line)
			{
				if (!foundWords.Contains(line))
				{   //no duplicates allowed
					foundWords.Add(line);
					Console.WriteLine("Match Found: " + line);
				}
			}
			//let this run through the whole dictionary to see if we should keep trying to build off of this word (plurals etc).
			//if we find one possibility here we can move on.
			if (line.Length > replaceQ.Length && line.Substring(0, replaceQ.Length) == replaceQ)
			{
				hasPossibleMatches = true;
			}
		}
		return hasPossibleMatches;
	}
}

