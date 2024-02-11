namespace cFlowForms;

public class StyledButton : Button
{
    // Constructor
    public StyledButton()
    {
        // Apply default styles
        ApplyStyles();
    }

    // Method to apply styles
    private void ApplyStyles()
    {
        // Set background color
        ((ButtonBase)this).BackColor = BackColor;
        ((ButtonBase)this).FlatStyle = FlatStyle.Flat;
        ((ButtonBase)this).FlatAppearance.BorderSize = 0;
        // Set text color
        ((Control)this).ForeColor = ForeColor;
    }

    public static readonly Color HighlightColor = Color.LightGray;
    public static readonly Color ForeColor = Color.DarkSlateGray;
    public static readonly Color BackColor = Color.DarkSlateGray;

}
