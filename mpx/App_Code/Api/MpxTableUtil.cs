using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.OleDb;

public static class MpxTableUtil
{
    public static string CheckTableName(string text)
    {
        var regex = "^[a-zA-Z0-9]*$";
        if (!System.Text.RegularExpressions.Regex.IsMatch(text, regex))
        {
            throw new Exception("Invalid table name");
        }
        return "[" + text + "]";
    }
    public static string CheckColumnName(string text)
    {
        var regex = "^[a-zA-Z0-9]*$";
        if (!System.Text.RegularExpressions.Regex.IsMatch(text, regex))
        {
            throw new Exception("Invalid table name");
        }
        return "[" + text + "]";
    }

    private static string CreateConnectionString(string source)
    {
        return "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + source + ";";
    }
    private static void Execute(string source, string sql, Action<OleDbCommand> action, Action<string> onError)
    {
        OleDbConnection connection = new OleDbConnection(CreateConnectionString(source));
        OleDbCommand cmd = new OleDbCommand(sql, connection);
        try
        {
            connection.Open();
            action(cmd);
            connection.Close();
        }
        catch (Exception ex)
        {
            onError(ex.Message);
        }
        finally
        {
            try
            {
                connection.Close();
            }
            catch (Exception) { }
        }
    }

    private static int GetNextId(string source, string tableName, string idColumn)
    {
        var sql = String.Format("SELECT max({0}) from {1}", CheckColumnName(idColumn), CheckTableName(tableName));
        var newId = -1;
        Execute(source, sql,
            (cmd) =>
            {
                int maxId = (int)cmd.ExecuteScalar();
                newId = maxId + 1;
            },
            (error) =>
            {
            }
        );
        return newId;
    }
    public static string AddRow(string source, string tableName, string idColumn, List<Tuple<string, string>> defaultValues, out int nextId)
    {
        var _nextId = GetNextId(source, tableName, idColumn);

        defaultValues.Add(new Tuple<string, string>(idColumn, "" + _nextId));

  
        var rowsToAdd = "";
        var valuesToAdd = "";
        for(var i = 0; i < defaultValues.Count; i++)
        {
            if (i > 0) { rowsToAdd += ", "; }
            rowsToAdd += CheckColumnName(defaultValues[i].Item1);
        }

        for (var i = 0; i < defaultValues.Count; i++)
        {
            if (i > 0) { valuesToAdd += ", "; }
            valuesToAdd += "?";
        }

        var sql = "INSERT INTO " + CheckTableName(tableName) + " (" + rowsToAdd + ") VALUES (" + valuesToAdd + ");";

        var result = "";
        Execute(source, sql,
            (cmd) =>
            {
                for (var i = 0; i < defaultValues.Count; i++)
                {
                    cmd.Parameters.AddWithValue("@" + defaultValues[i].Item1, defaultValues[i].Item2);
                }
               
                var numRowsAffected = cmd.ExecuteNonQuery();
                if (numRowsAffected != 1)
                {
                    result = "err|rows affected" + numRowsAffected;
                }
                else
                {
                    result = "ok|" + _nextId;
                }
            },
            (error) =>
            {
                result = "err|" + error;
                _nextId = -1;
            }
        );
        nextId = _nextId;
        return result;
    }

    public static string DeleteRow(string source, string tableName, string idColumn, string id)
    {
        var sql = String.Format("DELETE FROM {0} WHERE {1}=?", CheckTableName(tableName), CheckColumnName(idColumn));
        var result = "";
        Execute(source, sql,
            (cmd) =>
            {
                cmd.Parameters.AddWithValue("@id", id);
                var numRowsAffected = cmd.ExecuteNonQuery();
                if (numRowsAffected != 1)
                {
                    result = "err|rows affected" + numRowsAffected;
                }
                else
                {
                    result = "ok|" + id;
                }
            },
            (error) =>
            {
                result = "err|" + error;
            }
        );
        return result;
    }

    public static string UpdateRow(string source, string tableName, string idColumn, string id, string updateColumn, string newValue)
    {
        var sql = String.Format("UPDATE {0} SET {2}=? WHERE {1}=?",
           CheckTableName(tableName),
           CheckColumnName(idColumn),
           CheckColumnName(updateColumn));
        var result = "";
        Execute(source, sql,
            (cmd) =>
            {
                cmd.Parameters.AddWithValue("@newValue", newValue == null ? DBNull.Value : (object)newValue);
                cmd.Parameters.AddWithValue("@id", id);
                var numRowsAffected = cmd.ExecuteNonQuery();
                if (numRowsAffected != 1)
                {
                    result = "err|rows affected" + numRowsAffected;
                }
                else
                {
                    result = "ok|" + id;
                }
            },
            (error) =>
            {
                result = "err|" + error;
            }
        );
        return result;
    }

    public static TableResults SelectAll(string source, string tableName)
    {
        var sql = String.Format("SELECT * FROM {0}", CheckTableName(tableName));
        var result = new TableResults();
        Execute(source, sql,
            (cmd) =>
            {
                OleDbDataAdapter adapter = new OleDbDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                foreach (DataColumn c in dt.Columns)
                {
                    result.Columns.Add(c.ColumnName);
                }

                foreach (DataRow row in dt.Rows)
                {
                    var t = row;
                    result.Rows.Add(row.ItemArray);
                }
            },
            (error) =>
            {

            }
        );
        return result;
    }

    public static TableResults SelectList(string source, string tableName, string columnName)
    {
        string sql = String.Format("SELECT {1} FROM {0}", CheckTableName(tableName), CheckColumnName(columnName));

        var result = new TableResults();
        Execute(source, sql,
            (cmd) =>
            {
                OleDbDataAdapter adapter = new OleDbDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                foreach (DataColumn c in dt.Columns)
                {
                    result.Columns.Add(c.ColumnName);
                }

                foreach (DataRow row in dt.Rows)
                {
                    var t = row;
                    result.Rows.Add(row.ItemArray);
                }
            },
            (error) =>
            {

            }
        );
        return result;
    }
}