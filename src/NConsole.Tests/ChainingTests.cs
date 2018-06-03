﻿using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace NConsole.Tests
{
    public class ChainingTests
    {
        public class SumCommand : IConsoleCommand
        {
            [Argument(Name = "a")]
            public int A { get; set; }

            [Argument(Name = "b")]
            public int B { get; set; }

            public async Task<object> RunAsync(CommandLineProcessor processor, IConsoleHost host)
            {
                return A + B;
            }
        }

        public class SubtractCommand : IConsoleCommand
        {
            [Argument(Name = "a", AcceptsCommandInput = true)]
            public int A { get; set; }

            [Argument(Name = "b")]
            public int B { get; set; }

            public async Task<object> RunAsync(CommandLineProcessor processor, IConsoleHost host)
            {
                return A - B;
            }
        }

        [Fact]
        public async Task When_command_is_chained_then_output_is_passed_as_input_to_second_command()
        {
            //// Arrange
            var processor = new CommandLineProcessor(new ConsoleHost());
            processor.RegisterCommand<SumCommand>("sum");
            processor.RegisterCommand<SubtractCommand>("subtract");

            var args = new [] { "sum", "/a:6", "/b:10", "=", "subtract", "/b:7" };

            //// Act
            var result = await processor.ProcessAsync(args);

            //// Assert
            Assert.Equal(9, result.Last().Output);
        }

        [Fact]
        public async Task When_command_is_chained_and_output_is_ignored_then_first_command_does_not_influence_second_command()
        {
            //// Arrange
            var processor = new CommandLineProcessor(new ConsoleHost());
            processor.RegisterCommand<SumCommand>("sum");
            processor.RegisterCommand<SubtractCommand>("subtract");

            var args = new[] { "sum", "/a:6", "/b:10", "=", "subtract", "/a: 20", "/b:7" };

            //// Act
            var result = await processor.ProcessAsync(args);

            //// Assert
            Assert.Equal(13, result.Last().Output);
        }
    }
}
