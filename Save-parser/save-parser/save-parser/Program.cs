using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Buffers.Binary;
using CommandLine;
using Newtonsoft.Json;

class BinaryReader2 : BinaryReader
{
    public BinaryReader2(System.IO.Stream stream) : base(stream) { }

    public override Int16 ReadInt16()
    {
        var data = base.ReadBytes(2);
        Array.Reverse(data);
        return BitConverter.ToInt16(data, 0);
    }
}

class Program
{
    public class Options
    {
        [Option('f', "file", Required = true, HelpText = "Save file to parse")]
        public string File {get; set;}
    }

    public class Weapon
    {
        public int weaponId { get; set; }
        public string weaponName { get; set; }
        public int weaponDamageOffset { get; set; }
        public string weaponElement { get; set; }
        public int weaponSkills { get; set; }
    }

    public class DWOfficer
    {
        public string officerName { get; set; }
        //public int officerId { get; set; }
        public int officerLevel { get; set; }
        public int costume { get; set; }
        public Weapon weapon1 { get; set; }

    }


    static void Main(string[] args)
    {

        // To read data at specific byte: Address = Seek offset + number of bytes
        // i.e. to get data at offset 100 = Seek offset 99, as number of bytes is set to 1
        // if changing the size of bytes, adjust offset up or down
        // holy shit i finally figured it out
        // was using reader.read(), was giving me a random fucking idiot number.
        // even reversing the endianness was still giving out random shit
        // tried converting to int16/int32 AFTER read(), was still giving the same result. 
        // reserved the endianness WHILE ALSO reading it as an int16 gave out -26330, which is
        // 9981 in big endian. so i reversed and back and bam, 9881 returned
        // seems like officer XP could start at index 3052. previously it was around 4700
        // as i thought it started at zhao yun, but looks like it might start with the Wei officers
        // playable officers = 41, officer stat block = 168 bytes, 168*41 = 6888

        string file;
        

        Parser.Default.ParseArguments<Options>(args)
            .WithParsed<Options>(o =>
            {
                file = o.File;

                int numBytes = 4;
                byte[] test = new byte[numBytes];

                var officersDict = new Dictionary<int, string>()
                {
                    {0, "Xiahou Dun"},
                    {1, "Dian Wei"},
                    {2, "Sima Yi"},
                    {3, "Zhang Liao"},
                    {4, "Cao Cao"},
                    {5, "Zhou Yu"},
                    {6, "Lu Xun"},
                    {7, "Sun Shang Xiang"},
                    {8, "Gan Ning"},
                    {9, "Sun Jian"},
                    {10, "Zhao Yun"},
                    {11, "Guan Yu"},
                    {12, "Zhang Fei"},
                    {13, "Zhuge Liang"},
                    {14, "Liu Bei"},
                    {15, "Diao Chan"},
                    {16, "Lu Bu"},
                    {17, "Xu Zhu"},
                    {18, "Xiahou Yuan"},
                    {19, "Xu Huang"},
                    {20, "Zhang He"},
                    {21, "Cao Ren"},
                    {22, "Cao Pi"},
                    {23, "Taishi Ci"},
                    {24, "Lu Meng"},
                    {25, "Huang Gai"},
                    {26, "Zhou Tai"},
                    {27, "Ling Tong"},
                    {28, "Sun Ce"},
                    {29, "Sun Quan"},
                    {30, "Ma Chao"},
                    {31, "Huang Zhong"},
                    {32, "Wei Yan"},
                    {33, "Guan Ping"},
                    {34, "Pang Tong"},
                    {35, "Dong Zhuo"},
                    {36, "Yuan Shao"},
                    {37, "Zhang Jiao"},
                    {38, "Zhen Ji"},
                    {39, "Xiao Qiao"},
                    {40, "Yue Ying"}
                };
                var weaponsDict = new Dictionary<int, string>()
                {
                    { 00, "Rock Crusher"},
                    { 01, "Wave Breaker"},
                    { 02, "Thundersmash"},
                    { 03, "Violent Soul Flail"},
                    { 04, "Lion's Head Flail"},
                    { 05, "Beserker Flail"},
                    { 06, " Eradication Claws"},
                    { 07, "Anguish Claws"},
                    { 08, "Necrosis Claws"},
                    { 09, "Twin Vipers"},
                    { 10, "Twin Dragons"},
                    { 11, "Twin Eagles"},
                    { 12, "Sword of Heaven"},
                    { 13, "Blue Blade"},
                    { 14, "Seven Star Sword"},
                    { 15, "Red Dusk"},
                    { 16, "Dark Night"},
                    { 17, "Scarlet Dawn"},
                    { 18, "Silver Swallow"},
                    { 19, "Blue Falcon"},
                    { 20, "Jade Warbler"},
                    { 21, "Madder Rose"},
                    { 22, "Wisteria Breeze"},
                    { 23, "Lotus Bow"},
                    { 24, "Crescent Moon"},
                    { 25, "Dancing Dragon"},
                    { 26, "Wing Blade"},
                    { 27, "Elder Sword"},
                    { 28, "Nine Hook Sword"},
                    { 29, "Golden Pheonix"},
                    { 30, "Dragon Spike"},
                    { 31, "Dragon Fang"},
                    { 32, "Dragon Talon"},
                    { 33, "Blue Dragon"},
                    { 34, "Black Dragon"},
                    { 35, "White Dragon"},
                    { 36, "Serpent Blade"},
                    { 37, "Python Blade"},
                    { 38, "Viper Blade"},
                    { 39, "Brilliance"},
                    { 40, "Distinction"},
                    { 41, "Enlightenment"},
                    { 42, "Strength and Virtue"},
                    { 43, "Heaven and Earth"},
                    { 44, "Yin and Yang"},
                    { 45, "Moonflower"},
                    { 46, "Dewflower"},
                    { 47, "Rainflower"},
                    { 48, "Sky Piercer"},
                    { 49, "Demon Bane"},
                    { 50, "Heron Blade Halberd"},
                    { 51, "Bone Crusher"},
                    { 52, "Chaos Crusher"},
                    { 53, "Whirlwind Crusher"},
                    { 54, "Heavens Destroyer"},
                    { 55, "Heavens Smasher"},
                    { 56, "Heavens Cutter"},
                    { 57, "Destroyer"},
                    { 58, "Annihilator"},
                    { 59, "Obliterator"},
                    { 60, "Splendor"},
                    { 61, "Mystery"},
                    { 62, "Ostentation"},
                    { 63, "Phoenix Wing"},
                    { 64, "Dragon Scale"},
                    { 65, "Tortoise Bite"},
                    { 66, "Havoc"},
                    { 67, "Mayhem"},
                    { 68, "Chaos"},
                    { 69, "Rage Trident"},
                    { 70, "Savage Trident"},
                    { 71, "Tsunami Trident"},
                    { 72, "Valor"},
                    { 73, "Spirit"},
                    { 74, "Courage"},
                    { 75, "River Slicer"},
                    { 76, "Mountain Breaker"},
                    { 77, "Sky Lasher"},
                    { 78, "Flashstrike"},
                    { 79, "Dawnstrike"},
                    { 80, "Duskstrike"},
                    { 81, "Flying Nimbus"},
                    { 82, "Rising Nimbus"},
                    { 83, "Lofting Nimbus"},
                    { 84, "Tryant Strike"},
                    { 85, "Glimmer Strike"},
                    { 86, "Stoic Strike"},
                    { 87, "Dragon's Might"},
                    { 88, "Heaven's Might"},
                    { 89, "Tiatan's Might"},
                    { 90, "Ironhorse Glaive"},
                    { 91, "Dragonrun Glaive"},
                    { 92, "Warsteed Glaive"},
                    { 93, "Immortal Blade"},
                    { 94, "Battle Master Blade"},
                    { 95, "Princeps Blade"},
                    { 96, "The Awakener"},
                    { 97, "Bone Splitter"},
                    { 98, "Stormhowl"},
                    { 99, "Blue Dragon Ji"},
                    { 100, "Black Dragon Ji"},
                    { 101, "White Dragon Ji"},
                    { 102, "Firestorm Staff"},
                    { 103, "Blizzard Staff"},
                    { 104, "Typhoon Staff"},
                    { 105, "Wizard Club"},
                    { 106, "Magus Club"},
                    { 107, "Augur Club"},
                    { 108, "Sword of Kings"},
                    { 109, "Sword of Severity"},
                    { 110, "North Star Sword"},
                    { 111, "Blaze Staff"},
                    { 112, "Blight Staff"},
                    { 113, "Judgement Staff"},
                    { 114, "Allure"},
                    { 115, "Charm"},
                    { 116, "Seduction"},
                    { 117, "True"},
                    { 118, "True Beauty"},
                    { 119, "True Luster"},
                    { 120, "Emerald Dew"},
                    { 121, "Emerald Veil"},
                    { 122, "Emerald Mist"},
                    { 174, "Empty" }
                };

                var fighterDictionary = new Dictionary<string, DWOfficer>();
                
                using (BinaryReader2 reader = new BinaryReader2(new FileStream(file, FileMode.Open)))
                {
                    // real address in 010 editor minus 4
                    int emblemAddress = 269234;




                    int readerBuffer;


                    // create new item in dwofficerdictionary on each loop
                    // and new dwofficer 
                    //fighterDictionary.Add("fighter" + i, new DWOfficer());


                    Console.WriteLine(reader.BaseStream.Seek(emblemAddress, SeekOrigin.Begin));
                    reader.Read(test, 0, 4);
                    readerBuffer = reader.ReadInt16();
                    Console.WriteLine("emblem: " + readerBuffer);
                    //fighterDictionary["fighter" + i].officerName = officersDict[reader.ReadInt32()];
                    //DWOfficerDictionary["DWOfficer" + i].officerId = reader.ReadInt32();



                    //string json = JsonConvert.SerializeObject(fighterDictionary["fighter" + i], Formatting.Indented);
                    //Console.WriteLine(json);

                    //Console.WriteLine("********* \n\n");


                    //i = i + 167;
                    // 168 bytes per officer 
                    // fyi - save.dat bytes 2904 to 9791
                    // 6888 / 168 = 41 :)


                }

            });

        while (true) ;
    }

}

   

