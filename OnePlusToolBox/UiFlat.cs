using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Text;

namespace OnePlusToolBox
{
    public partial class UiFlat : Form
    {
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        private static extern IntPtr AddFontMemResourceEx(IntPtr pbFont, uint cbFont,
            IntPtr pdv, [System.Runtime.InteropServices.In] ref uint pcFonts);

        private PrivateFontCollection fonts = new PrivateFontCollection();

        Font titleFont;
        Font tabFont;
        public UiFlat()
        {
            InitializeComponent();

            // Reg Font
            byte[] fontData1 = Properties.Resources.OnePlusSans_Regular;
            IntPtr fontPtr1 = System.Runtime.InteropServices.Marshal.AllocCoTaskMem(fontData1.Length);
            System.Runtime.InteropServices.Marshal.Copy(fontData1, 0, fontPtr1, fontData1.Length);
            uint dummy1 = 0;
            fonts.AddMemoryFont(fontPtr1, Properties.Resources.OnePlusSans_Regular.Length);
            AddFontMemResourceEx(fontPtr1, (uint)Properties.Resources.OnePlusSans_Regular.Length, IntPtr.Zero, ref dummy1);
            System.Runtime.InteropServices.Marshal.FreeCoTaskMem(fontPtr1);

            titleFont = new Font(fonts.Families[0], 16F);
            tabFont = new Font(fonts.Families[0], 12F);
        }

        private void UiFlat_Load(object sender, EventArgs e)
        {
            title.Font = titleFont;
            label1.Font = tabFont;
            label2.Font = tabFont;
        }
    }
}
