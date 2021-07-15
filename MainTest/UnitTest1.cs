using Main;
using System.Text.RegularExpressions;
using Xunit;

namespace MainTest
{
    public class UnitTest1
    {
        [Fact]
        public void Check_for_odd_Pipes()
        {
            var txt = "Invaso|520.1|Chiusura cm|45|Pompa Ch|P1";

            var hits = Regex.Matches(txt, @"\|").Count;

            Assert.True(hits % 2 == 1);
        }

        [Fact]
        public void Left_works_with_long_string()
        {
            var txt = "pippo";
            Assert.True(txt.Left(4) == "pipp");
        }

        [Fact]
        public void Chop_cuts_last_char_with_long_string()
        {
            var txt = "pippo";
            Assert.True(txt.Chop() == "pipp");
        }

        [Fact]
        public void Chop_does_nothing_with_single_char_string()
        {
            var txt = "p";
            Assert.True(txt.Chop() == "p");
        }

        [Fact]
        public void LastChar_return_last_char_of_long_string()
        {
            var txt = "pippo";
            Assert.True(txt.LastChar() == "o");
        }

        [Fact]
        public void LastChar_returns_string_if_has_only_one_char()
        {
            var txt = "p";
            Assert.True(txt.LastChar() == txt);
        }
    }
}
