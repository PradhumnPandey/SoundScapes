using SoundScapes;
internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine(" --------------------------------------------------------------------------------------------------------------------");
        Console.WriteLine("                                         Welcome to Image - Audio Broadcast                                         ");
        Console.WriteLine(" --------------------------------------------------------------------------------------------------------------------");
        Console.WriteLine("| This console application can be used to convert a image to audio signal or a audio singal to image.                |");
        Console.WriteLine("| User can broadcast a image as a audio signal without need of any network or radio wave                             |");
        Console.WriteLine("| Messages can be shared in a local area using a audio device, without bearing the load of broadcasting head count   |");
        Console.WriteLine("| For any assistance or query mail at pradhumn.pandey8888@gmail.com                                                  |");
        Console.WriteLine(" --------------------------------------------------------------------------------------------------------------------");
        Console.WriteLine("Select number to choose");
        Console.WriteLine("Press 1. Image to Sound");
        Console.WriteLine("Press 2. Sound to Image");
        int option = Convert.ToInt32(Console.ReadLine());
        Console.WriteLine("Press 1. For Black &  White (Faster suitable for transmitting messages)");
        Console.WriteLine("Press 2. For Color Image(Slower suitable for color image)");
        int type = Convert.ToInt32(Console.ReadLine());
        if (type == 2) Console.WriteLine("This Feature is under development");
        else
        {
            Main main = new Main(Directory.GetCurrentDirectory() + "\\Input\\img.jpg", option,type);
        }
    }
}