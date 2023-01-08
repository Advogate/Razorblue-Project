using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace IPFiltering
{
    class Program
    {
        static void Main(string[] args)
        {
            Program pg = new Program();

            //at this stage you get the IPAdress table from the database using entity framework
            string[] ranges = new string[3];
            ranges[0] = "192.168.1.1";
            ranges[1] = "192.168.1.2/24";
            ranges[2] = "192.168.1.3";
            pg.CompareIPAddress("192.168.1.3",ranges);
            Console.WriteLine("Done");
            Console.Read();
        }

        public bool CompareIPAddress(string ip, string[] ranges)
        {
            // Convert the IP address to a IPAddress object
            IPAddress address = IPAddress.Parse(ip);

            // Iterate through the list of ranges
            foreach (string range in ranges)
            {
                // Check if the range is a single IP address
                if (IPAddress.TryParse(range, out IPAddress singleIp))
                {
                    // If the range is a single IP address, compare it to the supplied address
                    if (singleIp.Equals(address))
                    {
                        Console.WriteLine("true");
                        return true;
                    }
                }
                // If the range is not a single IP address, it might be a range in the form "start-end"
                else if (range.Contains("-"))
                {
                    // Split the range into start and end
                    string[] startEnd = range.Split("-");
                    IPAddress start = IPAddress.Parse(startEnd[0]);
                    IPAddress end = IPAddress.Parse(startEnd[1]);

                    // Convert the IP addresses to integers for comparison
                    long startInt = BitConverter.ToInt32(start.GetAddressBytes().Reverse().ToArray(), 0);
                    long endInt = BitConverter.ToInt32(end.GetAddressBytes().Reverse().ToArray(), 0);
                    long addressInt = BitConverter.ToInt32(address.GetAddressBytes().Reverse().ToArray(), 0);

                    // Check if the supplied address is within the range
                    if (addressInt >= startInt && addressInt <= endInt)
                    {
                        Console.WriteLine("true");
                        return true;
                    }
                }
                // If the range is not a single IP address or a range in the form "start-end", it might be a CIDR range
                else if (range.Contains("/"))
                {
                    // Convert the CIDR range to a subnet mask
                    IPAddress mask = ConvertCidrToSubnetMask(range);

                    // Convert the IP addresses to bytes for comparison
                    byte[] addressBytes = address.GetAddressBytes();
                    byte[] maskBytes = mask.GetAddressBytes();

                    // Check if the supplied address is within the range
                    if (Enumerable.Range(0, 4).All(i => (addressBytes[i] & maskBytes[i]) == maskBytes[i]))
                    {
                        Console.WriteLine("true");
                        return true;
                    }
                }
            }

            // If the supplied address does not match any of the ranges, return false
            Console.WriteLine("true");
            return false;
        }

        private IPAddress ConvertCidrToSubnetMask(string cidr)
        {
            int prefixLength = int.Parse(cidr.Split("/")[1]);
            byte[] mask = new byte[4];
            for (int i = 0; i < 4; i++)
            {
                if (prefixLength >= 8)
                {
                    mask[i] = 255;
                    prefixLength -= 8;
                }
                else
                {
                    mask[i] = (byte)(256 - (1 << (8 - prefixLength)));
                    prefixLength = 0;
                }
            }
            return new IPAddress(mask);
        }
    }
}
