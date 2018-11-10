using System;
using System.Windows.Forms;

public class DataGridViewCalendarColumn : DataGridViewColumn
{
    public DataGridViewCalendarColumn() : base(new DataGridViewCalendarCell())
    {
        //NB This is Used so That A Column can be tagged as DataGridViewColumn
        //Reflection Will Take Control
    }
}

public class DataGridViewCalendarCell : DataGridViewTextBoxCell
{
    public DataGridViewCalendarCell()
        : base()
    {
    }
}

