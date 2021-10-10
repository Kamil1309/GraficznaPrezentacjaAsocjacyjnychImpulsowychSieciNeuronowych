
namespace MathHelp
{
    public class CheckBounds
    {
        public static bool IsBetween(double testValue, double bound1, double bound2)
        {
            if (bound1 > bound2){
                return testValue >= bound2 && testValue <= bound1;
            }else{
                return testValue >= bound1 && testValue <= bound2;
            }
        }
    }
}
