using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOS_Lab2 {
	public class ProcessorModel {

		public ProcessorModel () {
			registers = new Dictionary<RegisterName, Register>() {
				{ RegisterName.IR, new CommandRegister(this) },
				{ RegisterName.PS, new StatusRegister(this) },
				{ RegisterName.PC, new ValueRegister(this) },
				{ RegisterName.TC, new ValueRegister(this) },
				{ RegisterName.R1, new ValueRegister(this) },
				{ RegisterName.R2, new ValueRegister(this) },
				{ RegisterName.R3, new ValueRegister(this) },
				{ RegisterName.R4, new ValueRegister(this) },
			};

			commands = new Dictionary<AssemblerCommand.OpCode, Command>() {
				{ AssemblerCommand.OpCode.Add, new AddCommand(this) },
				{ AssemblerCommand.OpCode.Neg, new NegCommand(this) },
				{ AssemblerCommand.OpCode.Mov, new MovCommand(this) },
			};
		}

		public override String ToString () {
			var sb = new StringBuilder();
			AddRegisterToString(sb, RegisterName.IR);
			AddRegisterToString(sb, RegisterName.R1);
			AddRegisterToString(sb, RegisterName.R2);
			AddRegisterToString(sb, RegisterName.R3);
			AddRegisterToString(sb, RegisterName.R4);
			sb.Append('\n');
			AddRegisterToString(sb, RegisterName.PS);
			AddRegisterToString(sb, RegisterName.PC);
			AddRegisterToString(sb, RegisterName.TC);

			return sb.ToString();
		}

		private void AddRegisterToString (StringBuilder sb, RegisterName rgName) {
			sb.Append($"{rgName.ToString().ToUpper()}: |   |");
			sb.Append(registers[rgName].ToString());
			sb.Append('\n');
		}

		public void SetProgram (AssemblerCommand[] commands) {
			this.programCommands = new List<AssemblerCommand>(commands);
			this.programCommands.RemoveAll((command) => command.IsEmpty || command.Name == AssemblerCommand.OpCode.None);

			curCommand = null;

			((CommandRegister)registers[RegisterName.IR]).SetValue(null);
			registers[RegisterName.PS].SetValue(new BitValue(new bool[BIT_LENGTH]));
			registers[RegisterName.PC].SetValue(new BitValue(new bool[BIT_LENGTH]));
			registers[RegisterName.TC].SetValue(new BitValue(new bool[BIT_LENGTH]));
		}

		public void MakeTick () {
			if (curCommand != null && curCommand.IsDone) {
				MoveToNextCommand();
			}

			int curCommandIndex = registers[RegisterName.PC].GetValue().ToInt();
			if (curCommandIndex >= programCommands.Count) return;

			if (curCommand == null) {
				registers[RegisterName.TC].SetValue(new BitValue(new bool[BIT_LENGTH]));				
				curCommand = InitiateCommand(programCommands[curCommandIndex]);
				((CommandRegister)registers[RegisterName.IR]).SetValue(programCommands[curCommandIndex]);
				return;
			}

			if (!curCommand.IsDone) {
				curCommand.MakeStep();
			}
		}

		public bool IsExecuting () {
			int commandIndex = registers[RegisterName.PC].GetValue().ToInt();
			return !(programCommands.Count - 1 == commandIndex && curCommand != null && curCommand.IsDone);
		}

		private void MoveToNextCommand () {
			curCommand = null;

			var one = new bool[BIT_LENGTH];
			one[0] = true;

			BitValue curCommandIndex = new BitValue(registers[RegisterName.PC].GetValue());
			curCommandIndex.Add(new BitValue(one));
			registers[RegisterName.PC].SetValue(new BitValue(curCommandIndex));
		}

		private Command InitiateCommand (AssemblerCommand assemblerCommand) {
			var targetCommand = commands[assemblerCommand.Name];
			targetCommand.UpdateCommand(assemblerCommand);
			return targetCommand;
		}

		public static readonly int BIT_LENGTH = 8;
		public readonly Dictionary<RegisterName, Register> registers;
		private readonly Dictionary<AssemblerCommand.OpCode, Command> commands;

		private List<AssemblerCommand> programCommands;
		private Command curCommand;
		
	}
}
