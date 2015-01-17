using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using System.Speech.Synthesis;

namespace Jarvis
{
    class Program
    {
        private static SpeechSynthesizer synth = new SpeechSynthesizer();

        // 
        //  WHERE ALL THE MAGIC HAPPENS!
        //  
        static void Main(string[] args)
        {
            // List of messages that will be selected at random when the CPU is hammered!
            //List<string> cpuMaxedOutMessages = new List<string>();
            //cpuMaxedOutMessages.Add("WARNING: Holy crap your CPU is about to overload!");
            //cpuMaxedOutMessages.Add("WARNING: oh my god you should not run your CPU that hard");
            //cpuMaxedOutMessages.Add("WARNING: Stop downloading the porn it's maxing me out");
            //cpuMaxedOutMessages.Add("WARNING: Your CPU is officially chasing squirrels");
            //cpuMaxedOutMessages.Add("RED ALERT! RED ALERT! RED ALERT! RED ALERT! I FARTED");

            // The dice! LIKE DND
            //Random rand = new Random();

            // This will greet the user in the default voice
            synth.Speak("Welcome to Jarvis Performance Notifier, version one point oh!");         
            
            #region My Performance Counters
            // This will pull the current CPU load in percentage
            PerformanceCounter perfCpuCount = new PerformanceCounter("Processor Information", "% Processor Time", "_Total");
            perfCpuCount.NextValue();

            // This will pull the current available memory in Megabytes
            PerformanceCounter perfMemCount = new PerformanceCounter("Memory", "Available MBytes");
            perfMemCount.NextValue();

            // This will get us the system uptime (in seconds)
            PerformanceCounter perfUptimeCount = new PerformanceCounter("System", "System Up Time");
            perfUptimeCount.NextValue();
            #endregion

            TimeSpan uptimeSpan = TimeSpan.FromSeconds(perfUptimeCount.NextValue());
            string systemUptimeMessage = string.Format("The current system up time is {0} days {1} hours {2} minutes {3} seconds",
                (int)uptimeSpan.TotalDays,
                (int)uptimeSpan.Hours,
                (int)uptimeSpan.Minutes,
                (int)uptimeSpan.Seconds
                );

            // Tell the user what the current system uptime is
            JerrySpeak(systemUptimeMessage, VoiceGender.Male, 2);

            int speechSpeed = 1;
            //bool isChromeOpenedAlready = false;

            // Infinite While Loop
            while(true)
            {
                // Get the current performance counter values
                int currentCpuPercentage = (int)perfCpuCount.NextValue();
                float currentAvailableMemory = perfMemCount.NextValue();

                // Every 1 second print the CPU load in percentage to the screen
                Console.WriteLine("CPU Load        : {0}%", currentCpuPercentage);
                if (currentCpuPercentage >= 80 && currentCpuPercentage <= 99)
                {
                    // Display a Warning that CPU Load is getting critically high
                    Console.WriteLine("Your CPU is about to overload, prepare to close down some programmes", currentCpuPercentage);
                }
                else if (currentCpuPercentage == 100)
                {
                    // Display a Message that CPU Load is maxed out
                    Console.WriteLine(" - Your CPU is overloaded, close down some programmes ASAP", currentCpuPercentage);
                }
                else
                {
                    Console.WriteLine(" - CPU is running nicely", currentCpuPercentage);
                }
                Console.WriteLine("Available Memory: {0}GB", currentAvailableMemory / 1024);
                if (currentAvailableMemory < 1024)
                {
                    // Display a Warning that Memory is low
                    Console.WriteLine(" - Available Memory at {0}MB is pretty low, it's time you shut some programs down", currentAvailableMemory);
                }
                else if (currentAvailableMemory >= 1024)
                {
                    // Display a Message that Memory is ok
                    Console.WriteLine(" - Available Memory of {0}GB is reasonable", currentAvailableMemory / 1024);
                }
                // Only tell us when the CPU is above 80% usage
                #region Logic
                if ( currentCpuPercentage > 80 )
                {
                    if (currentCpuPercentage >= 90 && currentCpuPercentage <= 99)
                    {
                        // This is designed to prevent the speech speed from exceeding 5x normal
                        string cpuLoadVocalMessage = String.Format("WARNING: Holy crap your CPU is about to overload!");                      
                        JerrySpeak(cpuLoadVocalMessage, VoiceGender.Male, speechSpeed);
                    }
                    else if (currentCpuPercentage == 100)
                    {
                        // This is designed to prevent the speech speed from exceeding 5x normal
                        string cpuLoadVocalMessage = String.Format("WARNING: Holy crap your CPU is overloaded!");
                        JerrySpeak(cpuLoadVocalMessage, VoiceGender.Male, speechSpeed);
                    }
                    else
                    {
                        string cpuLoadVocalMessage = String.Format("The current CPU load is {0} percent", currentCpuPercentage);
                        JerrySpeak(cpuLoadVocalMessage, VoiceGender.Female, 5);
                    }
                }
                #endregion

                // Only tell us when memory is below one gigabyte
                #region Logic
                if (currentAvailableMemory < 1024)
                {
                    // Speak to the user with text to speech to tell them what the current values are
                    string memAvailableVocalMessage = String.Format("You currently only have {0} megabytes of memory available, consider closing some programs as soon as possible", currentAvailableMemory);
                    JerrySpeak(memAvailableVocalMessage, VoiceGender.Male, 2);
                }
                //else if (currentAvailableMemory >= 1024)
                //{
                    // Speak to the user with text to speech to tell them what the current values are
                    //string memAvailableVocalMessage = String.Format("You currently have {0} gigabytes of memory available", currentAvailableMemory / 1024);
                    //JerrySpeak(memAvailableVocalMessage, VoiceGender.Male, 2);
                //}

                Thread.Sleep(30000);
            } // end of loop
                #endregion
        }

        /// <summary>
        /// Speaks with a selected voice
        /// </summary>
        /// <param name="message"></param>
        /// <param name="voiceGender"></param>
        public static void JerrySpeak(string message, VoiceGender voiceGender)
        {
            synth.SelectVoiceByHints(voiceGender);
            synth.Speak(message);
        }

        /// <summary>
        /// Speaks with a selected voice at a selected speed
        /// </summary>
        /// <param name="message"></param>
        /// <param name="voiceGender"></param>
        /// <param name="rate"></param>
        public static void JerrySpeak(string message, VoiceGender voiceGender, int rate)
        {
            synth.Rate = rate;
            JerrySpeak(message, voiceGender);
        }

        // Open a website
        public static void OpenWebsite(string URL)
        {
            Process p1 = new Process();
            p1.StartInfo.FileName = "chrome.exe";
            p1.StartInfo.Arguments = URL;
            p1.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
            p1.Start();
        }
    }
}
