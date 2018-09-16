namespace IKPokeEditor
{
    partial class NombrePokemon
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.PokemonName = new System.Windows.Forms.TextBox();
            this.AcceptName = new System.Windows.Forms.Button();
            this.CancelName = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(33, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(112, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Nombre del Pokémon:";
            // 
            // PokemonName
            // 
            this.PokemonName.Location = new System.Drawing.Point(36, 43);
            this.PokemonName.MaxLength = 12;
            this.PokemonName.Name = "PokemonName";
            this.PokemonName.Size = new System.Drawing.Size(141, 20);
            this.PokemonName.TabIndex = 1;
            // 
            // AcceptName
            // 
            this.AcceptName.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.AcceptName.Location = new System.Drawing.Point(36, 69);
            this.AcceptName.Name = "AcceptName";
            this.AcceptName.Size = new System.Drawing.Size(67, 23);
            this.AcceptName.TabIndex = 2;
            this.AcceptName.Text = "Aceptar";
            this.AcceptName.UseVisualStyleBackColor = true;
            this.AcceptName.Click += new System.EventHandler(this.AcceptName_Click);
            // 
            // CancelName
            // 
            this.CancelName.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelName.Location = new System.Drawing.Point(109, 69);
            this.CancelName.Name = "CancelName";
            this.CancelName.Size = new System.Drawing.Size(68, 23);
            this.CancelName.TabIndex = 3;
            this.CancelName.Text = "Cancelar";
            this.CancelName.UseVisualStyleBackColor = true;
            // 
            // NombrePokemon
            // 
            this.AcceptButton = this.AcceptName;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(211, 109);
            this.Controls.Add(this.CancelName);
            this.Controls.Add(this.AcceptName);
            this.Controls.Add(this.PokemonName);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "NombrePokemon";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Nombre del Pokémon";
            this.Load += new System.EventHandler(this.NombrePokemon_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox PokemonName;
        private System.Windows.Forms.Button AcceptName;
        private System.Windows.Forms.Button CancelName;
    }
}