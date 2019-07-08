using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;

/// <summary>
/// Summary description for GroupModelIO
/// </summary>
public class GroupModelIO
{
	public GroupModelIO()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public static void SaveGroupFileToDb(string filePath, string fileName, string fileOwner) {

        try {
            byte[] rawData = File.ReadAllBytes(filePath);
            int fileSize = rawData.Length;

            // other way to get a file to byte array
            //FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            //long fileSize2 = fs.Length;
            //rawData = new byte[fileSize];
            //fs.Read(rawData, 0, (int)fileSize2);
            //fs.Close();

            DbUse.RunMySqlParams("INSERT INTO webmpx.group_files (FileName, File, FileSize, File_owner) VALUES (@FileName, @File, @FileSize, @File_owner);",
                new string[] { "@FileName", "@File", "@FileSize", "@File_owner" },
                new object[] { fileName, rawData, fileSize, fileOwner });
        } catch (Exception ex) {
            // show error message...
            LogFiles logFiles = new LogFiles();
            logFiles.ErrorLog(ex);
        }

    }

    public static byte[] xxReadGroupFile(int fileId) {
        using (MySqlConnection conn = new MySqlConnection(DbUse.GetConnectionString())) {
            using (MySqlCommand cmd = new MySqlCommand("SELECT File From Webmpx.group_files WHERE file_id = @fileId;", conn)) {
                try {
                    conn.Open();
                    cmd.Parameters.AddWithValue("@fileId", fileId);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read()) {
                        byte[] fileBytes = (byte[])reader["File"];
                        conn.Close();
                        return fileBytes;
                    } else {
                        conn.Close();
                        throw new Exception("File not found in the database");
                    }
                    
                } catch (Exception ex) {
                    throw new Exception("Error in reading group file, sql query: " + cmd.CommandText + ". " + ex.Message);
                }
            }
        }
    }

    public static void SaveGroupFileToDisk(int fileId, string filePath) {

        using (MySqlConnection conn = new MySqlConnection(DbUse.GetConnectionString())) {
            using (MySqlCommand cmd = new MySqlCommand("SELECT File From Webmpx.group_files WHERE file_id = @fileId;", conn)) {
                try {
                    conn.Open();
                    cmd.Parameters.AddWithValue("@fileId", fileId);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read()) {
                        byte[] fileBytes = (byte[])reader["File"];
                        File.WriteAllBytes(filePath, fileBytes);
                    }
                    conn.Close();
                } catch (Exception ex) {
                    // show error message...
                    LogFiles logFiles = new LogFiles();
                    logFiles.ErrorLog(ex);
                }
            }
        }

    }
}