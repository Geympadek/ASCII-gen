
using System.Drawing;
using System.Numerics;
using ASCII_gen;
using Microsoft.VisualBasic;

internal class Program
{
    private static void Main()
    {
        GenInfo info = new();

        double scale = TryReadNumeric<double>("Enter scale of the image: ");

        var img = AsciiImg.Gen("E:\\Programming crap\\c#\\2024\\ASCII gen\\cat.jpg", info, scale);
        AsciiImg.Print(img);

        Console.ReadLine();
    }
    static T TryReadNumeric<T>(string msg) where T : INumber<T>
    {
        while (true)
        {
            Console.Write(msg);
            try
            {
                var result = Convert.ChangeType(Console.ReadLine(), typeof(T));
                if (result != null)
                {
                    return (T)result;
                }
            }
            catch (Exception) { }
            Console.WriteLine("Enter a valid value!");
        }
    }
}