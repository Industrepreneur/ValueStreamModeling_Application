using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for FileOperations
/// </summary>
public class FileOperations
{
	public FileOperations()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    private static int NUM_TRIES = 12;

    public static bool Read_File(string file_name) {

        string[] sBuffer;

        //  needs translation - just reading the whole file to insure it has been written by other code ....

        if (File.Exists(file_name)) {

            using(StreamReader fin = new StreamReader(file_name)) {

                sBuffer = File.ReadAllLines(file_name);

                fin.Close();

                return (true);
            }
        }
        return false;

    } // end function;





    public static bool Copy_File(string from_name, string to_name) {
        long size1;
        long size2;
        short ret;
        int i;
        int k;

        bool retval;

        retval = false;

        from_name = from_name.ToUpper();
        to_name = to_name.ToUpper();
        if (from_name.Equals(to_name)) {
            FileInfo fileInfo = new FileInfo(from_name);
            if (fileInfo.Exists) {
                retval = true;
            }
            return retval; //exit  Function;
        };



        

        try {
            FileInfo fileInfo = new FileInfo(from_name);
            size1 = (int)fileInfo.Length;
            File.Copy(from_name, to_name, true);
            FileInfo newFileInfo = new FileInfo(to_name);
            size2 = newFileInfo.Length;

            if ((size1 != size2)) {
                retval = false;
            };


            retval = Read_File(to_name);
        } catch (Exception) {
            
        }


        
        return retval;

    } // end function;

    public static bool WaitForFile(string fullPath) {
        return WaitForFile(fullPath, 500, NUM_TRIES);
    }

    public static bool WaitForFile(string fullPath, int milliseconds) {
        return WaitForFile(fullPath, milliseconds, 1);
    }

    public static bool WaitForFile(string fullPath, int milliseconds, int maxTries) {
        int numTries = 0;
        while (true) {
            ++numTries;
            try {
                // Attempt to open the file exclusively.
                using (FileStream fs = new FileStream(fullPath,
                    FileMode.Open, FileAccess.ReadWrite, FileShare.None, 100)) {
                    fs.ReadByte();

                    // If we got this far the file is ready
                    break;
                }
            } catch (Exception ex) {


                //logFiles.ErrorMessageLog("WaitForFile " + fullPath + "failed to get an exclusive lock: " + ex.ToString());

                if (numTries > maxTries) {
                    LogFiles logFiles = new LogFiles();
                    logFiles.ErrorMessageLog(
                        "WaitForFile " + fullPath + " failed to get an exclusive lock: " + ex.ToString() + ". Giving up after " + maxTries + " tries.");

                    return false;
                }

                // Wait for the lock to be released
                System.Threading.Thread.Sleep(milliseconds);
            }
        }

        LogFiles lFiles = new LogFiles();
        lFiles.ErrorMessageLog("WaitForFile " + fullPath + " returning true after " + numTries + " tries.");
        return true;
    }

}