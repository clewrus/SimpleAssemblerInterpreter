using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

namespace AOS_Lab2 {
	class Program {
		static void Main (string[] args) {
			var model = CreateProcessorModel();

			do {
				Console.Clear();
				model.MakeTick();
				Console.Write(model.ToString());
				Console.ReadKey(false);
			} while (model.IsExecuting());
		}

		static ProcessorModel CreateProcessorModel () {
			string sourceFileName = "../../AssemblerCode.txt";
			string[] inputLines = File.ReadAllLines(sourceFileName);

			var commands = ExtractCommands(inputLines);

			var model = new ProcessorModel();
			model.SetProgram(commands);

			return model;
		}

		static AssemblerCommand[] ExtractCommands (string[] inputLines) {
			var commands = new AssemblerCommand[inputLines.Length];
			for (int i = 0; i < inputLines.Length; i++) {
				commands[i] = new AssemblerCommand(inputLines[i]);
			}

			return commands;
		}
	}
}
