using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for RulesEngine
/// </summary>
public class RulesEngine
{
    public RulesEngine(string newVal, string columnName, bool toUpper = true)
    {
        if(newVal == null)
        {
           newVal = "";
        }
        this.newVal = newVal.Trim();
        if(toUpper)
        {
            this.newVal = this.newVal.ToUpper();
        }
        this.columnName = columnName.ToLower();
    }

    private string newVal = null;   
    private string columnName = null;
    private string checkingColumnName = null;

    public bool HasError = false;
    public string Error = "";

    public RulesEngine checkColumn(string checkingColumnName)
    {
        this.checkingColumnName = checkingColumnName.ToLower();
        return this;
    }

    private bool IsOkToCheck()
    {
        if(this.HasError) { return false; }
        if (this.columnName != this.checkingColumnName) { return false; }
        return true;
    }
    public RulesEngine required()
    {
        if (!IsOkToCheck()) { return this; }
        if(this.newVal == null || this.newVal.Length == 0)
        {
            SetError("Required");
        }
        return this;
    }
    public RulesEngine integer()
    {
        if(!IsOkToCheck()) { return this; }

        int newValueInt = 0;
        if (!int.TryParse(this.newVal, out newValueInt))
        {
            SetError("Must be an integer");
        }

        return this;
    }
    public RulesEngine number()
    {
        if (!IsOkToCheck()) { return this; }

        double newValueDouble = 0;
        if (!double.TryParse(this.newVal, out newValueDouble))
        {
            SetError("Must be a number");
        }

        return this;
    }
    private double getAsNumber()
    {
        double newValueDouble = 0;
        if (!double.TryParse(this.newVal, out newValueDouble))
        {
            SetError("Must be a number");
        }
        return newValueDouble;
    }

    public RulesEngine greaterThan(double val)
    {
        if (!IsOkToCheck()) { return this; }

        double num = getAsNumber();
        if(!(num > val))
        {
            SetError("Must be more than " + val);
        }

        return this;
    }
    public RulesEngine greaterThanOrEqual(double val)
    {
        if (!IsOkToCheck()) { return this; }

        double num = getAsNumber();
        if (!(num >= val))
        {
            SetError("Must be equal or more than " + val);
        }

        return this;
    }

    public RulesEngine lessThan(double val)
    {
        if (!IsOkToCheck()) { return this; }

        double num = getAsNumber();
        if (!(num < val))
        {
            SetError("Must be less than " + val);
        }

        return this;
    }
    public RulesEngine lessThanOrEqual(double val)
    {
        if (!IsOkToCheck()) { return this; }

        double num = getAsNumber();
        if (!(num <= val))
        {
            SetError("Must be equal or less than " + val);
        }

        return this;
    }
    public RulesEngine inRange(double min, double max)
    {
        if (!IsOkToCheck()) { return this; }

        double num = getAsNumber();
        if (!(num >= min && num <= max))
        {
            SetError("Must be between " + min  + " and " + max);
        }

        return this;
    }

    // Special mpx check
    public RulesEngine negativeOneOrGreaterThanZero()
    {
        if (!IsOkToCheck()) { return this; }

        double num = getAsNumber();
        if (!(num == -1 || num > 0))
        {
            SetError("Must be -1 or more than 0");
        }

        return this;
    }

    public RulesEngine isSpecialMpxMath()
    {
        if (!IsOkToCheck()) { return this; }
        // TODO: actually check if can be parsed
        var regex = @"^[0-9\-\+\/\*\(\)\.\s]*$";
        if (!System.Text.RegularExpressions.Regex.IsMatch(this.newVal, regex))
        {
            SetError("Not a valid math function");
        }
        return this;
    }

    public RulesEngine exclude(string badVal)
    {
        if (!IsOkToCheck()) { return this; }
        if (string.Equals(this.newVal, badVal, StringComparison.OrdinalIgnoreCase)) {
            SetError("Cannot be " + badVal);
        }
        return this;
    }
    //public RulesEngine exclude(string[] badVals)
    //{
    //    if (!IsOkToCheck()) { return this; }
    //    for (var i = 0; i < badVals.Length; i++)
    //    {
    //        if (string.Equals(this.newVal, badVals[i], StringComparison.OrdinalIgnoreCase))
    //        {
    //            SetError("Cannot be " + badVals[i]);
    //        }
    //    }
    //    return this;
    //}

    public RulesEngine SetError(string error)
    {
        if (!this.HasError)
        {
            this.Error = error;
            this.HasError = true;
        }
        return this;
    }
}

