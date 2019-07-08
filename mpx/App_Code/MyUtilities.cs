using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for MyUtilities
/// </summary>
public static class MyUtilities {
    public const string NEWLINE = "\r\n";

    // add some other characters (nonvisible) !!!!!!!!
    // private static char[] SPECIAL_CHARACTERS = { '#', '<', '>', '\'', '/', '\\', '"', '&', '+', ':', '.', ' ', '!', '@' };

    private static char[] SPECIAL_CHARACTERS = { '#', '<', '>', '\'', '\\', '"', '&', ':', ';', '@' };


    public static int[] ZOOM_LEVELS = new int[] { 100, 75, 60, 50, 40, 25 };

    public const string POLES_IMG_NAME = "ipoles.png";

    public const string TREES_IMG_NAME = "itrees.png";

    public const string GRAPH_DEFAULT_NAME = "ChartPic_XXXXXX";

    public static string clean(string text) {

        string result = text;
        foreach (char ch in SPECIAL_CHARACTERS) {
            result = clean(result, ch);
        }
        return result;

    }

    public static string clean(string text, char ch) {
        string result = text;
        if(result == null) { result = ""; }
        int i = result.IndexOf(ch);
        while (i > -1) {
            result = result.Substring(0, i) + result.Substring(i + 1);
            i = result.IndexOf(ch);
        }
        return result;
    }

    public static void MsgBox(string text) {
        System.Web.HttpContext.Current.Response.Write("<script>alert('" + text + "')</script>");
    }

    public static double RoundNum(double num, int precission) {
        double helpNum = num * Math.Pow(10, precission);
        helpNum = Math.Round(helpNum);
        return helpNum / Math.Pow(10, precission);
    }

    public static int GetNumOfChars(string text, char ch) {
        string result = text;
        int count = 0;
        int i = result.IndexOf(ch);
        while (i > -1) {
            result = result.Substring(0, i) + result.Substring(i + 1);
            i = result.IndexOf(ch);
            count++;
        }
        return count;
    }

    public static double GetMaxDouble(double[] nums) {
        double max = Double.NegativeInfinity;
        foreach (double num in nums) {
            if (num > max) {
                max = num;
            }
        }
        return max;
    }

    public static float GetMaxFloat(float[] nums) {
        float max = float.NegativeInfinity;
        foreach (float num in nums) {
            if (num > max) {
                max = num;
            }
        }
        return max;
    }
}