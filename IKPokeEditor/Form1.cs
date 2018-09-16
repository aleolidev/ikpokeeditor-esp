using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace IKPokeEditor
{
    public partial class Form1 : Form
    {

        string directory = null;
        string mayusPokemonName;
        string minusPokemonName;
        string firstMayusPokemonName;
        string fileName = null;
        string pathErr = null;
        string fileErr = null;

        //Dictionaries
        Dictionary<string, string> dictionary = new Dictionary<string, string>();
        Dictionary<string, string> data = new Dictionary<string, string>();
        Dictionary<string, Dictionary<string, string>> pokemonData = new Dictionary<string, Dictionary<string, string>>();
        Dictionary<string, Dictionary<string, string>> infoData = new Dictionary<string, Dictionary<string, string>>();
        Dictionary<string, Tuple<string, string, string, string>> evolutionData = new Dictionary<string, Tuple<string, string, string, string>>();
        Dictionary<string, Tuple<string, string, string>> moveData = new Dictionary<string, Tuple<string, string, string>>();
        Dictionary<string, Tuple<string, string>> mtmoData = new Dictionary<string, Tuple<string, string>>();

        public Form1()
        {
            InitializeComponent();
        }

        private void seleccionarCarpetaToolStripMenuItem_Click(object sender, EventArgs e)
        {

            CommonOpenFileDialog FBD = new CommonOpenFileDialog();
            FBD.IsFolderPicker = true;
            FBD.RestoreDirectory = true;

            if (FBD.ShowDialog() == CommonFileDialogResult.Ok)
            {
                cleanAll();
                fileName = FBD.FileName;
                directory = FBD.FileName;
                bool checkResult = checkFilesExists();

                if (checkResult == true)
                {
                    //MessageBox.Show("Archivo encontrado");
                    setDataDictionary();
                    setPokemonDataDictionary();
                    setInfoDataDictionary();
                    loadData();
                    guardarToolStripMenuItem.Enabled = true;

                    comboBox1.SelectedIndex = 1;
                    comboBox2.SelectedIndex = 1;
                    comboBox3.SelectedIndex = 1;
                } else
                {
                    MessageBox.Show("No se ha encontrado el archivo " + fileErr + " en el directorio " + pathErr);
                }
            }
        }

        private bool checkFilesExists()
        {
            var rValue = true;
            var pathToSearch = "";

            string[] filePaths = {
                (directory.ToString() + "\\data\\graphics\\pokemon\\back_pic_table.inc"),
                (directory.ToString() + "\\data\\graphics\\pokemon\\front_pic_table.inc"),
                (directory.ToString() + "\\data\\graphics\\pokemon\\back_pic_coords.inc"),
                (directory.ToString() + "\\data\\graphics\\pokemon\\front_pic_coords.inc"),
                (directory.ToString() + "\\data\\graphics\\pokemon\\graphics.inc"),
                (directory.ToString() + "\\data\\graphics\\pokemon\\palette_table.inc"),
                (directory.ToString() + "\\data\\graphics\\pokemon\\shiny_palette_table.inc"),
                (directory.ToString() + "\\include\\constants\\species.h"),
                (directory.ToString() + "\\include\\global.h"),
                (directory.ToString() + "\\include\\graphics.h"),
                (directory.ToString() + "\\include\\pokedex.h"),
                (directory.ToString() + "\\sound\\direct_sound_data.inc"),
                (directory.ToString() + "\\sound\\voice_groups.inc"),
                (directory.ToString() + "\\src\\battle\\battle_1.c"),
                (directory.ToString() + "\\src\\data\\pokemon\\base_stats.h"),
                (directory.ToString() + "\\src\\data\\pokemon\\cry_ids.h"),
                (directory.ToString() + "\\src\\data\\pokemon\\level_up_learnset_pointers.h"),
                (directory.ToString() + "\\src\\data\\pokemon\\level_up_learnsets.h"),
                (directory.ToString() + "\\src\\data\\pokemon\\tmhm_learnsets.h"),
                (directory.ToString() + "\\src\\data\\text\\species_names_en.h"),
                (directory.ToString() + "\\src\\data\\pokedex_entries_en.h"),
                (directory.ToString() + "\\src\\data\\pokedex_orders.h"),
                (directory.ToString() + "\\src\\pokedex.c"),
                (directory.ToString() + "\\src\\pokemon_1.c"),
                (directory.ToString() + "\\src\\pokemon_icon.c"),
                (directory.ToString() + "\\src\\data\\text\\move_names_en.h"),
                (directory.ToString() + "\\src\\data\\items_en.h"),
                (directory.ToString() + "\\include\\pokemon.h"),
                (directory.ToString() + "\\include\\constants\\abilities.h"),
                (directory.ToString() + "\\src\\data\\pokemon\\evolution.h"),
            };

            List<string> folderPathsList = new List<string>();
            /*string[] folderPaths =
            {
                (directory.ToString() + "\\graphics\\pokemon"),
                (directory.ToString() + "\\graphics\\pokemon_icon_palettes"),
            };*/

            folderPathsList.Add(directory.ToString() + "\\graphics\\pokemon");
            folderPathsList.Add(directory.ToString() + "\\graphics\\pokemon_icon_palettes");

            if (Directory.Exists((directory.ToString() + "\\sound\\cries")))
            {
                folderPathsList.Add((directory.ToString() + "\\sound\\cries"));
            } else if (Directory.Exists((directory.ToString() + "\\sound\\direct_sound_samples\\cries")))
            {
                folderPathsList.Add((directory.ToString() + "\\sound\\direct_sound_samples\\cries"));
            }

            for (int i = 0; i < filePaths.Length; i++)
            {
                if (File.Exists(filePaths[i]) == false)
                {
                    rValue = false;
                    pathErr = filePaths[i].Substring(0, filePaths[i].LastIndexOfAny(new char[] { '\\', '/' }));
                    fileErr = filePaths[i].Substring(filePaths[i].LastIndexOfAny(new char[] { '\\', '/' }) + 1);
                }
                pathToSearch = filePaths[i];

                pathToSearch = pathToSearch.Substring(pathToSearch.LastIndexOfAny(new char[] { '\\', '/' }) + 1);
                pathToSearch = pathToSearch.Substring(0, pathToSearch.LastIndexOfAny(new char[] { '.' })) + "_" + pathToSearch.Substring(pathToSearch.LastIndexOfAny(new char[] { '.' }) + 1);

                dictionary["pFile_" + pathToSearch.ToString()] = filePaths[i];
                data["pFile_" + pathToSearch.ToString()] = null;

                //MessageBox.Show("pFile_" + pathToSearch + " directory: " + dictionary["pFile_" + pathToSearch.ToString()].ToString());

            }

            for (int i = 0; i < folderPathsList.Count; i++)
            {
                if (Directory.Exists(folderPathsList[i]) == false)
                {
                    rValue = false;
                    pathErr = folderPathsList[i];
                }

                pathToSearch = folderPathsList[i];

                pathToSearch = pathToSearch.Substring(pathToSearch.LastIndexOfAny(new char[] { '\\', '/' }) + 1);

                dictionary["pFolder_" + pathToSearch.ToString()] = folderPathsList[i];

                //MessageBox.Show("pFolder_" + pathToSearch + " directory: " + dictionary["pFolder_" + pathToSearch.ToString()].ToString());

                //MessageBox.Show("Folder: " + pathToSearch);
            }

            return rValue;
        }

        private void loadData()
        {
            loadPokemonBaseStats();
            loadPokemonNames();
            loadPokemonEvolutions();
            loadPokemonMovements();
            loadPokemonMTMO();
            loadDexData();
            loadSpriteData();
            //addArgumentsData();
            addMovementsData();
            addMTMOData();
        }

        private void loadPokemonNames()
        {
            var str = data["pFile_species_h"];
            int pastValue = 0;
            int pokeAmount = 0;
            string pokeName = "";

            pokeAmount = Regex.Matches(str, "#define SPECIES_").Cast<Match>().Count() - 29;

            comboBox1.Items.Clear();
            comboBox2.Items.Clear();
            comboBox3.Items.Clear();
            Evolucion.Items.Clear();

            for (int i = 0; i <= pokeAmount; i++)
            {
                int index = str.IndexOf("#define SPECIES_", pastValue + 1);
                pastValue = index;
                pokeName = str.Substring(index + 16, str.IndexOf(" ", index + 9) - index - 16);
                if (pokeName.ToLower().Contains('_')) {
                    pokeName = pokeName.Replace(@"_", " ");
                }
                comboBox1.Items.Insert(i, pokeName);
                comboBox2.Items.Insert(i, pokeName);
                comboBox3.Items.Insert(i, pokeName);
                Evolucion.Items.Insert(i, pokeName);
            }
        }

        private void loadPokemonBaseStats()
        {
            var str = data["pFile_base_stats_h"];
            var speciesNames = data["pFile_species_names_en_h"];
            var pokeAmount = 0;
            var index = 0;
            var pastValue = 0;
            var indexName = 0;
            var pastValueName = 0;
            string psBase = null;
            string ataqueBase = null;
            string defensaBase = null;
            string velocidadBase = null;
            string ataqueEspecialBase = null;
            string defensaEspecialBase = null;
            string type1 = null;
            string type2 = null;
            string ratioCaptura = null;
            string expBase = null;
            string evsPS = null;
            string evsAtaque = null;
            string evsDefensa = null;
            string evsVelocidad = null;
            string evsAtaqueEspecial = null;
            string evsDefensaEspecial = null;
            string objetoUno = null;
            string objetoDos = null;
            string genero = null;
            string ciclosHuevo = null;
            string amistadBase = null;
            string crecimiento = null;
            string grupoHuevoUno = null;
            string grupoHuevoDos = null;
            string habilidadUno = null;
            string habilidadDos = null;
            string probabilidadHuidaSafari = null;
            string colorCuerpo = null;
            string pokemonName = null;

            pokeAmount = Regex.Matches(str, "SPECIES_").Cast<Match>().Count() - 1;

            pastValue = str.IndexOf("SPECIES_", 0);
            pastValueName = speciesNames.IndexOf("SPECIES_", 0);
            //MessageBox.Show(pastValue.ToString());

            for (int i = 0; i < pokeAmount; i++)
            {
                index = str.IndexOf("SPECIES_", pastValue + 2);
                pastValue = index;

                var siguienteCorchete = str.IndexOf("]", index + 1) - index;
                psBase = str.Substring(index + siguienteCorchete + 35, (str.IndexOf(",", index + siguienteCorchete + 35)) - (index + siguienteCorchete + 35));
                pokemonData["psBase"][(i + 1).ToString()] = psBase;

                index = (str.IndexOf(",", index + siguienteCorchete + 35));
                ataqueBase = str.Substring((str.IndexOf("baseAttack", index)) + 16, (str.IndexOf(",", (str.IndexOf("baseAttack", index)))) - ((str.IndexOf("baseAttack", index)) + 16));
                pokemonData["ataqueBase"][(i + 1).ToString()] = ataqueBase;

                index = (str.IndexOf(",", (str.IndexOf("baseAttack", index)))) + 2;
                defensaBase = str.Substring(((str.IndexOf("baseDefense", index)) + 16), ((str.IndexOf(",", (str.IndexOf("baseDefense", index))))) - ((str.IndexOf("baseDefense", index)) + 16));
                pokemonData["defensaBase"][(i + 1).ToString()] = defensaBase;

                index = (str.IndexOf(",", (str.IndexOf("baseDefense", index)))) + 2;
                velocidadBase = str.Substring((str.IndexOf("baseSpeed", index) + 16), (str.IndexOf(",", index)) - (str.IndexOf("baseSpeed", index) + 16));
                pokemonData["velocidadBase"][(i + 1).ToString()] = velocidadBase;

                index = (str.IndexOf(",", index)) + 2;
                ataqueEspecialBase = str.Substring((str.IndexOf("baseSpAttack", index) + 16), (str.IndexOf(",", index)) - (str.IndexOf("baseSpAttack", index) + 16));
                pokemonData["ataqueEspecialBase"][(i + 1).ToString()] = ataqueEspecialBase;

                index = (str.IndexOf(",", index)) + 2;
                defensaEspecialBase = str.Substring((str.IndexOf("baseSpDefense", index) + 16), str.IndexOf(",", index) - (str.IndexOf("baseSpDefense", index) + 16));
                pokemonData["defensaEspecialBase"][(i + 1).ToString()] = defensaEspecialBase;

                index = (str.IndexOf(",", index)) + 2;
                type1 = str.Substring((str.IndexOf("type1", index) + 8), str.IndexOf(",", index) - (str.IndexOf("type1", index) + 8));
                pokemonData["tipoUno"][(i + 1).ToString()] = type1;

                index = (str.IndexOf(",", index)) + 2;
                type2 = str.Substring((str.IndexOf("type2", index) + 8), str.IndexOf(",", index) - (str.IndexOf("type2", index) + 8));
                pokemonData["tipoDos"][(i + 1).ToString()] = type2;

                index = (str.IndexOf(",", index)) + 2;
                ratioCaptura = str.Substring((str.IndexOf("catchRate", index) + 12), str.IndexOf(",", index) - (str.IndexOf("catchRate", index) + 12));
                pokemonData["ratioDeCaptura"][(i + 1).ToString()] = ratioCaptura;

                index = (str.IndexOf(",", index)) + 2;
                expBase = str.Substring((str.IndexOf("expYield", index) + 11), str.IndexOf(",", index) - (str.IndexOf("expYield", index) + 11));
                pokemonData["expBase"][(i + 1).ToString()] = expBase;

                index = (str.IndexOf(",", index)) + 2;
                evsPS = str.Substring((str.IndexOf("evYield_HP", index) + 20), (str.IndexOf(",", index)) - (str.IndexOf("evYield_HP", index) + 20));
                pokemonData["evsPS"][(i + 1).ToString()] = evsPS;

                index = (str.IndexOf(",", index)) + 2;
                evsAtaque = str.Substring((str.IndexOf("evYield_Attack", index) + 20), (str.IndexOf(",", index)) - (str.IndexOf("evYield_Attack", index) + 20));
                pokemonData["evsAtaque"][(i + 1).ToString()] = evsAtaque;

                index = (str.IndexOf(",", index)) + 2;
                evsDefensa = str.Substring((str.IndexOf("evYield_Defense", index) + 20), (str.IndexOf(",", index)) - (str.IndexOf("evYield_Defense", index) + 20));
                pokemonData["evsDefensa"][(i + 1).ToString()] = evsDefensa;

                index = (str.IndexOf(",", index)) + 2;
                evsVelocidad = str.Substring((str.IndexOf("evYield_Speed", index) + 20), (str.IndexOf(",", index)) - (str.IndexOf("evYield_Speed", index) + 20));
                pokemonData["evsVelocidad"][(i + 1).ToString()] = evsVelocidad;

                index = (str.IndexOf(",", index)) + 2;
                evsAtaqueEspecial = str.Substring((str.IndexOf("evYield_SpAttack", index) + 20), (str.IndexOf(",", index)) - (str.IndexOf("evYield_SpAttack", index) + 20));
                pokemonData["evsAtaqueEspecial"][(i + 1).ToString()] = evsAtaqueEspecial;

                index = (str.IndexOf(",", index)) + 2;
                evsDefensaEspecial = str.Substring((str.IndexOf("evYield_SpDefense", index) + 20), (str.IndexOf(",", index)) - (str.IndexOf("evYield_SpDefense", index) + 20));
                pokemonData["evsDefensaEspecial"][(i + 1).ToString()] = evsDefensaEspecial;

                index = (str.IndexOf(",", index)) + 2;
                objetoUno = str.Substring((str.IndexOf("item1", index) + 8), (str.IndexOf(",", index)) - (str.IndexOf("item1", index) + 8));
                pokemonData["objetoUno"][(i + 1).ToString()] = objetoUno;

                index = (str.IndexOf(",", index)) + 2;
                objetoDos = str.Substring((str.IndexOf("item2", index) + 8), (str.IndexOf(",", index)) - (str.IndexOf("item2", index) + 8));
                pokemonData["objetoDos"][(i + 1).ToString()] = objetoDos;

                index = (str.IndexOf(",", index)) + 2;
                genero = str.Substring((str.IndexOf("genderRatio", index) + 14), (str.IndexOf(",", index)) - (str.IndexOf("genderRatio", index) + 14));
                if (genero != "MON_GENDERLESS")
                {
                    if (genero == "MON_FEMALE")
                    {
                        genero = "100";
                    } else if (genero == "MON_MALE")
                    {
                        genero = "0";
                    } else
                    {
                        //MessageBox.Show(((str.IndexOf(")", index + 3)) - (index + 15)).ToString());
                        genero = str.Substring((str.IndexOf("genderRatio", index) + 29), (str.IndexOf(",", index)) - (str.IndexOf("genderRatio", index) + 29) - 1);
                    }
                    pokemonData["tieneGenero"][(i + 1).ToString()] = "true";
                } else
                {
                    pokemonData["tieneGenero"][(i + 1).ToString()] = "false";
                    genero = "0";
                }
                pokemonData["ratioGenero"][(i + 1).ToString()] = genero;

                index = (str.IndexOf(",", index)) + 2;
                ciclosHuevo = str.Substring((str.IndexOf("eggCycles", index) + 12), str.IndexOf(",", index) - (str.IndexOf("eggCycles", index) + 12));
                pokemonData["ciclosHuevo"][(i + 1).ToString()] = ciclosHuevo;

                index = (str.IndexOf(",", index)) + 2;
                amistadBase = str.Substring((str.IndexOf("friendship", index) + 13), str.IndexOf(",", index) - (str.IndexOf("friendship", index) + 13));
                pokemonData["amistadBase"][(i + 1).ToString()] = amistadBase;

                index = (str.IndexOf(",", index)) + 2;
                crecimiento = str.Substring((str.IndexOf("growthRate", index) + 13), str.IndexOf(",", index) - (str.IndexOf("growthRate", index) + 13));
                pokemonData["crecimiento"][(i + 1).ToString()] = crecimiento;

                index = (str.IndexOf(",", index)) + 2;
                grupoHuevoUno = str.Substring((str.IndexOf("eggGroup1", index) + 12), str.IndexOf(",", index) - (str.IndexOf("eggGroup1", index) + 12));
                pokemonData["grupoHuevoUno"][(i + 1).ToString()] = grupoHuevoUno;

                index = (str.IndexOf(",", index)) + 2;
                grupoHuevoDos = str.Substring((str.IndexOf("eggGroup2", index) + 12), str.IndexOf(",", index) - (str.IndexOf("eggGroup2", index) + 12));
                pokemonData["grupoHuevoDos"][(i + 1).ToString()] = grupoHuevoDos;

                index = (str.IndexOf(",", index)) + 2;
                habilidadUno = str.Substring((str.IndexOf("ability1", index) + 11), str.IndexOf(",", index) - (str.IndexOf("ability1", index) + 11));
                pokemonData["habilidadUno"][(i + 1).ToString()] = habilidadUno;

                index = (str.IndexOf(",", index)) + 2;
                habilidadDos = str.Substring((str.IndexOf("ability2", index) + 11), str.IndexOf(",", index) - (str.IndexOf("ability2", index) + 11));
                pokemonData["habilidadDos"][(i + 1).ToString()] = habilidadDos;

                index = (str.IndexOf(",", index)) + 2;
                probabilidadHuidaSafari = str.Substring((str.IndexOf("safariZoneFleeRate", index) + 21), str.IndexOf(",", index) - (str.IndexOf("safariZoneFleeRate", index) + 21));
                pokemonData["probabilidadHuidaSafari"][(i + 1).ToString()] = probabilidadHuidaSafari;

                index = (str.IndexOf(",", index)) + 2;
                colorCuerpo = str.Substring((str.IndexOf("bodyColor", index) + 12), str.IndexOf(",", index) - (str.IndexOf("bodyColor", index) + 12));
                pokemonData["colorCuerpo"][(i + 1).ToString()] = colorCuerpo;

                indexName = (speciesNames.IndexOf("SPECIES_", pastValueName + 2));
                pastValueName = indexName;
                pokemonName = speciesNames.Substring((speciesNames.IndexOf("_(", indexName)) + 3, (speciesNames.IndexOf(",", indexName)) - ((speciesNames.IndexOf("_(", indexName)) + 3) - 2);
                pokemonData["pokemonName"][(i + 1).ToString()] = pokemonName;
            }

        }

        private void loadPokemonEvolutions()
        {
            var str = data["pFile_evolution_h"];
            var index = 0;
            var evoIndex = 0;
            var evoAmount = 0;
            var pokemonIndex = "0";
            string workString = null;
            var pokemonToEvolve = "";
            var evoMethod = "";
            var argument = "";
            var evolution = "";

            var evolveAmount = Regex.Matches(str, @"\[SPECIES").Cast<Match>().Count() - 1;
            index = str.IndexOf("[SPECIES_", 0);
            //MessageBox.Show(index.ToString());

            //MessageBox.Show(pokeAmount.ToString());

            for (int i = 0; i <= evolveAmount; i++)
            {
                if (i != evolveAmount)
                {
                    workString = str.Substring(index, (str.IndexOf("[SPECIES_", (index + 1)) - index));
                } else
                {
                    //MessageBox.Show(index.ToString());
                    workString = str.Substring(index, (str.IndexOf("};", index) - index));
                }
                index = str.IndexOf("[SPECIES_", (index + 2));

                evoAmount = Regex.Matches(workString, "EVO_").Cast<Match>().Count();

                for (int j = 0; j < evoAmount; j++)
                {
                    if (evoIndex == 0) {
                        pokemonToEvolve = workString.Substring((workString.IndexOf("[SPECIES_", evoIndex) + 9), (workString.IndexOf("]", evoIndex) - (workString.IndexOf("[SPECIES_", evoIndex) + 9)));
                        evoIndex = (workString.IndexOf("[SPECIES_", evoIndex));
                    }
                    //MessageBox.Show(pokemonToEvolve);
                    evoMethod = workString.Substring(workString.IndexOf("EVO_", evoIndex), (workString.IndexOf(",", evoIndex) - workString.IndexOf("EVO_", evoIndex)));
                    evoIndex = workString.IndexOf("EVO_", evoIndex);
                    var firstComma = workString.IndexOf(",", evoIndex);
                    argument = workString.Substring((firstComma + 2), workString.IndexOf(",", firstComma + 1) - (firstComma + 2));
                    evoIndex = workString.IndexOf(",", firstComma + 1);
                    evolution = workString.Substring(evoIndex + 2, workString.IndexOf("}", evoIndex) - (evoIndex + 2));
                    evoIndex = workString.IndexOf("},", evoIndex) + 2;
                    if (j == (evoAmount - 1)) { evoIndex = 0; }

                    if (pokemonToEvolve == "NIDORAN_F")
                    {
                        pokemonIndex = "29";
                    } else if (pokemonToEvolve == "NIDORAN_M")
                    {
                        pokemonIndex = "32";
                    } else
                    {
                        pokemonIndex = (pokemonData["pokemonName"].FirstOrDefault(x => x.Value.Contains(pokemonToEvolve)).Key);
                    }

                    evolutionData[pokemonIndex + "_" + j.ToString()] = Tuple.Create(evoAmount.ToString(), evoMethod, argument, evolution);

                }

            }

        }

        private void loadPokemonMovements()
        {
            var str = data["pFile_level_up_learnsets_h"];
            var index = 0;
            var moveIndex = 0;
            var moveAmount = 0;
            string workString = null;

            var pokemonAmount = Regex.Matches(str, "LevelUpLearnset").Cast<Match>().Count();
            index = str.IndexOf("LevelUpLearnset", 0);

            for (int i = 0; i < pokemonAmount; i++)
            {
                workString = str.Substring(str.IndexOf("LEVEL_UP_MOVE", index), str.IndexOf("LEVEL_UP_END", index) - str.IndexOf("LEVEL_UP_MOVE", index));
                index = str.IndexOf("LEVEL_UP_MOVE", str.IndexOf("LEVEL_UP_END", index));
                moveAmount = Regex.Matches(workString, "LEVEL_UP_MOVE").Cast<Match>().Count();

                moveIndex = 0;

                for (int j = 0; j < moveAmount; j++)
                {
                    var movementLevel = workString.Substring((workString.IndexOf("LEVEL_UP_MOVE(", moveIndex) + 14), workString.IndexOf(",", moveIndex) - (workString.IndexOf("LEVEL_UP_MOVE(", moveIndex) + 14));
                    movementLevel = movementLevel.Replace(@" ", "");
                    moveIndex = workString.IndexOf(",", moveIndex) + 1;
                    var movementName = workString.Substring((workString.IndexOf("MOVE_", moveIndex)), (workString.IndexOf(",", moveIndex) - 1) - (workString.IndexOf("MOVE_", moveIndex)));
                    moveIndex = workString.IndexOf(",", moveIndex) + 1;
                    //MessageBox.Show(movementName + " at level " + movementLevel);
                    moveData[(i + 1).ToString() + "_" + j.ToString()] = Tuple.Create(moveAmount.ToString(), movementLevel, movementName);
                }
                //MessageBox.Show(moveAmount.ToString());
            }
        }

        private void loadPokemonMTMO()
        {
            var str = data["pFile_tmhm_learnsets_h"];
            var index = 0;
            var moveIndex = 0;
            var moveAmount = 0;
            string workString = null;

            var pokemonAmount = Regex.Matches(str, @"\[SPECIES_").Cast<Match>().Count() - 1;
            index = str.IndexOf("[SPECIES_", 0) + 1;
            index = str.IndexOf("[SPECIES_", index) - 1;

            for (int i = 0; i < pokemonAmount; i++)
            {
                workString = str.Substring((str.IndexOf("[SPECIES_", index)), ((str.IndexOf(",", index)) - (str.IndexOf("[SPECIES_", index))));
                index = str.IndexOf(",", index) + 1;
                moveAmount = Regex.Matches(workString, @"TMHM\(").Cast<Match>().Count();
                //richTextBox1.Text += workString + Environment.NewLine + Environment.NewLine;

                moveIndex = 0;
                for (int j = 0; j < moveAmount; j++)
                {
                    var movementName = workString.Substring(workString.IndexOf("TMHM(", moveIndex), workString.IndexOf(")", moveIndex) - workString.IndexOf("TMHM(", moveIndex));
                    movementName = movementName.Substring(movementName.IndexOf("(", 0) + 1);
                    moveIndex = workString.IndexOf(")", moveIndex) + 1;

                    mtmoData[(i + 1).ToString() + "_" + j.ToString()] = Tuple.Create(moveAmount.ToString(), movementName);
                    //MessageBox.Show(movementName);
                }

            }

        }

        private void loadDexData()
        {
            loadDescriptions();
            loadGeneralDexData();
        }

        private void loadDescriptions()
        {
            var str = data["pFile_pokedex_entries_en_h"];
            var index = 0;
            var stringIndex = 0;
            var pokemonSpecie = "";
            var pokemonIndex = 0;
            var pokemonAmount = (Regex.Matches(str, "\n\nstatic const").Cast<Match>().Count() + Regex.Matches(str, "#else\nstatic const").Cast<Match>().Count()) + 2;

            for (int i = 0; i < pokemonAmount; i++)
            {
                stringIndex = 0;

                if (str.IndexOf("\n\nstatic const", index + 1) > 0) {
                    if (str.IndexOf("\n\nstatic const", index + 10) > str.IndexOf("#else\nstatic const", index + 10))
                    {
                        index = str.IndexOf("#else\nstatic const", index) + 5;

                    } else
                    {
                        index = str.IndexOf("\n\nstatic const", index) + 1;
                    }
                } else
                {
                    index = str.IndexOf("#else\nstatic const", index) + 5;
                }


                var workString = str.Substring(index, (str.IndexOf(");", str.IndexOf("static const", index + 10)) - index + 2));
                pokemonSpecie = workString.Substring((workString.IndexOf("DexDescription_", 0) + "DexDescription_".Length), workString.IndexOf("_1", 0) - (workString.IndexOf("DexDescription_", 0) + "DexDescription_".Length));
                pokemonSpecie = Regex.Replace(pokemonSpecie, @"(\p{Lu})", " $1").TrimStart();
                if (pokemonSpecie == "Mrmime") { pokemonSpecie = "Mr Mime"; }
                pokemonSpecie = pokemonSpecie.ToUpper();

                var descriptionOne = workString.Substring((workString.IndexOf("_1[] = _(", stringIndex) + 10), (workString.IndexOf(");", stringIndex)) - (workString.IndexOf("_1[] = _(", stringIndex) + 10));
                stringIndex = workString.IndexOf(");", stringIndex) + 2;
                var descrpitionTwo = workString.Substring((workString.IndexOf("_2[] = _(", stringIndex) + 10), workString.IndexOf(");", stringIndex) - (workString.IndexOf("_2[] = _(", stringIndex) + 10));

                descriptionOne = descriptionOne.Replace("\\n", "");
                descriptionOne = descriptionOne.Replace("\n", " ");
                descriptionOne = descriptionOne.Replace("\"", "");
                descriptionOne = descriptionOne.Replace("  ", "");
                descrpitionTwo = descrpitionTwo.Replace("\\n", "");
                descrpitionTwo = descrpitionTwo.Replace("\n", " ");
                descrpitionTwo = descrpitionTwo.Replace("\"", "");
                descrpitionTwo = descrpitionTwo.Replace("  ", "");

                if (comboBox1.Items.Contains(pokemonSpecie))
                {
                    pokemonIndex = comboBox1.Items.IndexOf(pokemonSpecie);
                }

                pokemonData["pokedexPageOne"][pokemonIndex.ToString()] = descriptionOne;
                pokemonData["pokedexPageTwo"][pokemonIndex.ToString()] = descrpitionTwo;
            }
        }

        private void loadGeneralDexData()
        {
            var str = data["pFile_pokedex_entries_en_h"];
            var index = 0;
            var stringIndex = 0;
            var pokemonIndex = 0;
            string pokemonName = "";
            string categoria = "";
            string altura = "";
            string peso = "";
            string escalaPokemon = "";
            string offsetPokemon = "";
            string escalaEntrenador = "";
            string offsetEntrenador = "";
            var strPal = data["pFile_pokemon_icon_c"];

            var pokemonAmount = Regex.Matches(str, ".categoryName").Cast<Match>().Count() - 1;
            var pokemonAmount2 = Regex.Matches(strPal, "gMonIcon").Cast<Match>().Count() - 1;

            index = str.IndexOf(".categoryName", 0) + 1;
            index = str.IndexOf("},", index) + 2;

            for (int i = 0; i < pokemonAmount; i++)
            {
                var workString = str.Substring(str.IndexOf("{", index), str.IndexOf("},", index) - str.IndexOf("{", index) + 2);
                index = str.IndexOf("},", index) + 2;
                stringIndex = 0;
                pokemonName = workString.Substring((workString.IndexOf("DexDescription_", 0) + 15), workString.IndexOf("_1", 0) - (workString.IndexOf("DexDescription_", 0) + 15));
                pokemonName = Regex.Replace(pokemonName, @"(\p{Lu})", " $1").TrimStart();
                if (pokemonName == "Mrmime") { pokemonName = "Mr Mime"; }
                pokemonName = pokemonName.ToUpper();

                if (comboBox1.Items.Contains(pokemonName))
                {
                    pokemonIndex = comboBox1.Items.IndexOf(pokemonName);
                }

                categoria = workString.Substring((workString.IndexOf("categoryName", stringIndex) + 18), workString.IndexOf("\"),", stringIndex) - (workString.IndexOf("categoryName", stringIndex) + 18));
                pokemonData["categoriaPokemon"][pokemonIndex.ToString()] = categoria;
                stringIndex = workString.IndexOf("\"),", stringIndex) + 4;

                altura = workString.Substring((workString.IndexOf("height", stringIndex) + 9), workString.IndexOf(",", stringIndex) - (workString.IndexOf("height", stringIndex) + 9));
                pokemonData["altura"][pokemonIndex.ToString()] = altura;
                stringIndex = workString.IndexOf(",", stringIndex) + 2;

                peso = workString.Substring((workString.IndexOf("weight", stringIndex) + 9), workString.IndexOf(",", stringIndex) - (workString.IndexOf("weight", stringIndex) + 9));
                pokemonData["peso"][pokemonIndex.ToString()] = peso;
                stringIndex = workString.IndexOf("pokemonScale", stringIndex);

                escalaPokemon = workString.Substring((workString.IndexOf("pokemonScale", stringIndex) + 15), workString.IndexOf(",", stringIndex) - (workString.IndexOf("pokemonScale", stringIndex) + 15));
                pokemonData["escalaPokemon"][pokemonIndex.ToString()] = escalaPokemon;
                stringIndex = workString.IndexOf(",", stringIndex) + 2;

                offsetPokemon = workString.Substring((workString.IndexOf("pokemonOffset", stringIndex) + 16), workString.IndexOf(",", stringIndex) - (workString.IndexOf("pokemonOffset", stringIndex) + 16));
                pokemonData["offsetPokemon"][pokemonIndex.ToString()] = offsetPokemon;
                stringIndex = workString.IndexOf(",", stringIndex) + 2;

                escalaEntrenador = workString.Substring((workString.IndexOf("trainerScale", stringIndex) + 15), workString.IndexOf(",", stringIndex) - (workString.IndexOf("trainerScale", stringIndex) + 15));
                pokemonData["escalaEntrenador"][pokemonIndex.ToString()] = escalaEntrenador;
                stringIndex = workString.IndexOf(",", stringIndex) + 2;

                offsetEntrenador = workString.Substring((workString.IndexOf("trainerOffset", stringIndex) + 16), workString.IndexOf(",", stringIndex) - (workString.IndexOf("trainerOffset", stringIndex) + 16));
                pokemonData["offsetEntrenador"][pokemonIndex.ToString()] = offsetEntrenador;
                stringIndex = workString.IndexOf(",", stringIndex) + 2;
            }
            index = strPal.IndexOf("const u8 gMonIconPaletteIndices[] =", 0);
            index = strPal.IndexOf("{", index);
            for (int i = 0; i < pokemonAmount2; i++)
            {
                index = strPal.IndexOf("\n", index) + 5;
                var workString = strPal.Substring(index, strPal.IndexOf(",", index) - index);
                pokemonData["palUsed"][i.ToString()] = workString;
            }
        }

        private void loadSpriteData()
        {
            var bCString = data["pFile_back_pic_coords_inc"];
            var fCString = data["pFile_front_pic_coords_inc"];
            var elevateString = data["pFile_battle_1_c"];
            var index = 0;

            var backBytes = Regex.Matches(bCString, ".byte").Cast<Match>().Count() - 1;
            var frontBytes = Regex.Matches(fCString, ".byte").Cast<Match>().Count() - 1;
            var elevateCount = Regex.Matches(elevateString, @"\[SPECIES_").Cast<Match>().Count();
            var palAmount = Directory.GetFiles(dictionary["pFolder_pokemon_icon_palettes"], "*.pal", SearchOption.AllDirectories).Length;

            for (int i = 0; i < backBytes; i++)
            {
                index = bCString.IndexOf(".byte", index + 1);
                var backY = (bCString.Substring((bCString.IndexOf(",", index) + 1), (bCString.IndexOf(",", bCString.IndexOf(",", index) + 1)) - (bCString.IndexOf(",", index) + 1))).Replace(" ", "");
                pokemonData["backCord"][i.ToString()] = backY.ToString();
            }
            index = 0;
            for (int i = 0; i < frontBytes; i++)
            {
                index = fCString.IndexOf(".byte", index + 1);
                var frontY = (fCString.Substring((fCString.IndexOf(",", index) + 1), (fCString.IndexOf(",", fCString.IndexOf(",", index) + 1)) - (fCString.IndexOf(",", index) + 1))).Replace(" ", "");
                pokemonData["frontCord"][i.ToString()] = frontY.ToString();
            }
            index = 0;
            for (int i = 0; i < elevateCount; i++)
            {
                index = elevateString.IndexOf("[SPECIES_", index) + 9;
                //var pokemonName = elevateString.Substring(index, elevateString.IndexOf("]", index) - index);
                var elevation = elevateString.Substring((elevateString.IndexOf("=", index) + 1), (elevateString.IndexOf(",", index) - (elevateString.IndexOf("=", index) + 1)));
                //MessageBox.Show("POKéMON: " + pokemonName + "\nElevation: " + elevation);
                pokemonData["elevate"][(i + 1).ToString()] = elevation.ToString();
            }
            for (int i = 0; i < palAmount; i++) 
            {
                iconPalette.Items.Add(i.ToString());
            }

        }

        private void debuggerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            getSizeValue(fondo.Image);
        }

        private string getSizeValue(Image imagen)
        {
            if (imagen != null)
            {
                Bitmap b = new Bitmap(imagen);
            
                bool found = false;
                Point firstY = new Point(0, 0);
                Point lastY = new Point(0, 0);
                Point firstX = new Point(0, 0);
                Point lastX = new Point(0, 0);
                int realX;
                int realY;
                //GET FIRST COLOR AT Y
                for (int x = 0; x < imagen.Width; x++)
                {
                    for (int y = 0; y < imagen.Height; y++)
                    {
                        if (found == false)
                        {
                            if (b.GetPixel(x, y).A != 0)
                            {
                                firstY = new Point(x, y);
                                found = true;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                found = false;
                //GET LAST COLOR AT Y
                for (int x = imagen.Width - 1; x >= 0; x--)
                {
                    for (int y = 0; y < imagen.Height; y++)
                    {
                        if (found == false)
                        {
                            if (b.GetPixel(x, y).A != 0)
                            {
                                lastY = new Point(x + 1, y + 1);
                                found = true;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                found = false;
                //GET FIRST COLOR AT X
                for (int y = 0; y < imagen.Height; y++)
                {
                    for (int x = 0; x < imagen.Width; x++)
                    {
                        if (found == false)
                        {
                            if (b.GetPixel(x, y).A != 0)
                            {
                                firstX = new Point(x, y);
                                found = true;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                found = false;
                //GET LAST COLOR AT X
                for (int y = imagen.Height - 1; y >= 0; y--)
                {
                    for (int x = 0; x < imagen.Width; x++)
                    {
                        if (found == false)
                        {
                            if (b.GetPixel(x, y).A != 0)
                            {
                                lastX = new Point(x + 1, y + 1);
                                found = true;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                realX = (lastY.X - firstY.X);
                realY = (lastX.Y - firstX.Y);
                if ((realX % 8) != 0)
                {
                    realX += (8 - realX % 8);
                }
                if ((realY % 8) != 0)
                {
                    realY += (8 - realY % 8);
                }
                var tileX = realX / 8;
                var tileY = realY / 8;
                var binX = Convert.ToString(Convert.ToInt64(tileX), 2);
                var binY = Convert.ToString(Convert.ToInt64(tileY), 2);

                if (binX.Length < 4)
                {
                    for (int i = 0; i < (4 - binX.Length); i++)
                    {
                        binX = "0" + binX;
                    }
                }
                if (binY.Length < 4)
                {
                    for (int i = 0; i < (4 - binY.Length); i++)
                    {
                        binY = "0" + binY;
                    }
                }

                var binString = binX + binY;
                binString = (Convert.ToInt32(binString, 2)).ToString();

                //MessageBox.Show((Convert.ToInt32(binString, 2)).ToString());

                //watch.Stop();
                //var elapsedMs = watch.ElapsedMilliseconds;
                //MessageBox.Show("Real size\n\nx: " + realX + "\ny: " + realY + "\n\nTiempo transcurrido: " + elapsedMd);

                b.Dispose();

                return binString;
                //MessageBox.Show(colour.ToString());
            } else
            {
                return "0";
            }
        }



        private void Form1_Load(object sender, EventArgs e)
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            pictureBox2.Parent = fondo;
            pictureBox3.Parent = fondo;
            pictureBox2.BackColor = Color.Transparent;
            pictureBox3.BackColor = Color.Transparent;
        }

        private void guardarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveData();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            generarPokemon();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            saveData();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            saveData();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            saveData();
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((comboBox1.SelectedIndex == 0) && (comboBox1.Text == "NONE")) {
                MessageBox.Show("No es posible cargar los datos de \"NONE\"");
                comboBox1.SelectedIndex = 1;
            } else
            {
                comboBox2.SelectedIndex = comboBox1.SelectedIndex;
                comboBox3.SelectedIndex = comboBox1.SelectedIndex;
                refrescarInterfaz();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if ((dataGridView2.Rows.Count - 1) > 0) {
                dataGridView2.Rows.RemoveAt(Int32.Parse(EvolucionAEliminar.Value.ToString()));
            }
            if ((Int32.Parse(EvolucionAEliminar.Value.ToString())) == (Int32.Parse(evolutionData[comboBox1.SelectedIndex.ToString() + "_0"].Item1) - 1))
            {
                if ((dataGridView2.Rows.Count - 1) > 0 && (EvolucionAEliminar.Value > 0)) {
                    EvolucionAEliminar.Value--;
                }
            }
            if (dataGridView2.Rows.Count > 1) {
                EvolucionAEliminar.Maximum = (dataGridView2.Rows.Count - 2);
            } else
            {
                EvolucionAEliminar.Maximum = 0;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if ((dataGridView1.Rows.Count - 1) > 1)
            {
                dataGridView1.Rows.RemoveAt(Int32.Parse(MovimientoAEliminar.Value.ToString()));
            }
            if ((Int32.Parse(MovimientoAEliminar.Value.ToString())) == (Int32.Parse(moveData[comboBox1.SelectedIndex.ToString() + "_0"].Item1) - 1))
            {
                MovimientoAEliminar.Value--;
            }
            MovimientoAEliminar.Maximum = (dataGridView1.Rows.Count - 2);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if ((dataGridView3.Rows.Count - 1) > 0)
            {
                dataGridView3.Rows.RemoveAt(Int32.Parse(MTMOAEliminar.Value.ToString()));
            }
            if ((Int32.Parse(MTMOAEliminar.Value.ToString())) == (Int32.Parse(moveData[comboBox1.SelectedIndex.ToString() + "_0"].Item1) - 1))
            {
                MTMOAEliminar.Value--;
            }
            if ((dataGridView3.Rows.Count - 1) > 0)
            {
                MTMOAEliminar.Maximum = (dataGridView3.Rows.Count - 2);
            }
        }

        private void frontY_ValueChanged(object sender, EventArgs e)
        {
            setSpritePosition();
        }

        private void backY_ValueChanged(object sender, EventArgs e)
        {
            setSpritePosition();
        }

        private void Levitation_ValueChanged(object sender, EventArgs e)
        {
            setSpritePosition();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((comboBox2.SelectedIndex == 0) && (comboBox1.Text == "NONE"))
            {
                MessageBox.Show("No es posible cargar los datos de \"NONE\"");
                comboBox1.SelectedIndex = 1;
            }
            else
            {
                comboBox3.SelectedIndex = comboBox2.SelectedIndex;
                comboBox1.SelectedIndex = comboBox2.SelectedIndex;
            }

        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((comboBox2.SelectedIndex == 0) && (comboBox1.Text == "NONE"))
            {
                MessageBox.Show("No es posible cargar los datos de \"NONE\"");
                comboBox1.SelectedIndex = 1;
            }
            else
            {
                comboBox2.SelectedIndex = comboBox3.SelectedIndex;
                comboBox1.SelectedIndex = comboBox3.SelectedIndex;
            }
        }

        private void dataGridView3_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView3.Rows.Count > 1)
            {
                MTMOAEliminar.Maximum = dataGridView3.Rows.Count - 2;
            }
            else
            {
                MTMOAEliminar.Maximum = 0;
            }
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.Rows.Count > 1)
            {
                MovimientoAEliminar.Maximum = dataGridView1.Rows.Count - 2;
            }
            else
            {
                MovimientoAEliminar.Maximum = 0;
            }
        }

        private void dataGridView2_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView2.Rows.Count > 1)
            {
                EvolucionAEliminar.Maximum = dataGridView2.Rows.Count - 2;
            }
            else
            {
                EvolucionAEliminar.Maximum = 0;
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (tabControl1.SelectedIndex)
            {
                case 0:
                    this.Size = new Size(1027, 619);
                    break;
                case 1:
                    this.Size = new Size(411, 525);
                    break;
                case 2:
                    this.Size = new Size(512, 356);
                    break;

            }
        }

        private void generoCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (generoCheck.Checked == true)
            {
                genero.Enabled = true;
            }
            else
            {
                genero.Enabled = false;
            }
        }

        private void detectIfNumber(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsDigit(e.KeyChar) || (e.KeyChar == (char)Keys.Back)))
                e.Handled = true;
        }

        private bool checkIfNumeric(string someString)
        {
            foreach (char c in someString)
            {
                if (!char.IsDigit(c))
                    return false;
            }
            return true;
        }

        private void detectIfNumberAndDecimal(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsDigit(e.KeyChar) || (e.KeyChar == (char)Keys.Back) || (e.KeyChar == '.')))
                e.Handled = true;
        }

        private void refrescarInterfaz()
        {
            PS_Base.Text = pokemonData["psBase"][comboBox1.SelectedIndex.ToString()];
            ATQ_Base.Text = pokemonData["ataqueBase"][comboBox1.SelectedIndex.ToString()];
            DEF_Base.Text = pokemonData["defensaBase"][comboBox1.SelectedIndex.ToString()];
            VEL_Base.Text = pokemonData["velocidadBase"][comboBox1.SelectedIndex.ToString()];
            ATESP_Base.Text = pokemonData["ataqueEspecialBase"][comboBox1.SelectedIndex.ToString()];
            DFESP_Base.Text = pokemonData["defensaEspecialBase"][comboBox1.SelectedIndex.ToString()];
            //Recibir tipo 1
            //TIPO1.SelectedIndex = Int32.Parse(infoData["tipos"].FirstOrDefault(x => x.Value.Contains(pokemonData["tipoUno"][comboBox1.SelectedIndex.ToString()].Substring(5))).Key);
            var formatoTipo1 = pokemonData["tipoUno"][comboBox1.SelectedIndex.ToString()].Substring(5);
            formatoTipo1 = formatoTipo1.Replace(@"_", " ");
            TIPO1.SelectedIndex = Int32.Parse(infoData["tipos"].FirstOrDefault(x => x.Value.Contains(formatoTipo1)).Key);
            //Recibir tipo 2
            //TIPO2.SelectedIndex = Int32.Parse(infoData["tipos"].FirstOrDefault(x => x.Value.Contains(pokemonData["tipoDos"][comboBox1.SelectedIndex.ToString()].Substring(5))).Key);
            var formatoTipo2 = pokemonData["tipoDos"][comboBox1.SelectedIndex.ToString()].Substring(5);
            formatoTipo2 = formatoTipo2.Replace(@"_", " ");
            TIPO2.SelectedIndex = Int32.Parse(infoData["tipos"].FirstOrDefault(x => x.Value.Contains(formatoTipo2)).Key);

            ratioCaptura.Text = pokemonData["ratioDeCaptura"][comboBox1.SelectedIndex.ToString()];
            expBase.Text = pokemonData["expBase"][comboBox1.SelectedIndex.ToString()];
            PS_Effort.Text = pokemonData["evsPS"][comboBox1.SelectedIndex.ToString()];
            ATQ_Effort.Text = pokemonData["evsAtaque"][comboBox1.SelectedIndex.ToString()];
            DEF_Effort.Text = pokemonData["evsDefensa"][comboBox1.SelectedIndex.ToString()];
            VEL_Effort.Text = pokemonData["evsVelocidad"][comboBox1.SelectedIndex.ToString()];
            ATESP_Effort.Text = pokemonData["evsAtaqueEspecial"][comboBox1.SelectedIndex.ToString()];
            DFESP_Effort.Text = pokemonData["evsDefensaEspecial"][comboBox1.SelectedIndex.ToString()];
            //Recibir objeto 1
            var formatoObjeto1 = pokemonData["objetoUno"][comboBox1.SelectedIndex.ToString()].Substring(5);
            formatoObjeto1 = formatoObjeto1.Replace(@"_", " ");
            OBJETO1.SelectedIndex = Int32.Parse(infoData["objetos"].FirstOrDefault(x => x.Value.Contains(formatoObjeto1)).Key);
            //Recibir objeto 2
            var formatoObjeto2 = pokemonData["objetoDos"][comboBox1.SelectedIndex.ToString()].Substring(5);
            formatoObjeto2 = formatoObjeto2.Replace(@"_", " ");
            OBJETO2.SelectedIndex = Int32.Parse(infoData["objetos"].FirstOrDefault(x => x.Value.Contains(formatoObjeto2)).Key);

            genero.Text = pokemonData["ratioGenero"][comboBox1.SelectedIndex.ToString()];
            if (pokemonData["tieneGenero"][comboBox1.SelectedIndex.ToString()] == "true")
            {
                generoCheck.Checked = true;
            } else if (pokemonData["tieneGenero"][comboBox1.SelectedIndex.ToString()] == "false")
            {
                generoCheck.Checked = false;
            }
            ciclosHuevo.Text = pokemonData["ciclosHuevo"][comboBox1.SelectedIndex.ToString()];
            amistadBase.Text = pokemonData["amistadBase"][comboBox1.SelectedIndex.ToString()];
            //Recibir crecimiento
            var formatoCrecimiento = pokemonData["crecimiento"][comboBox1.SelectedIndex.ToString()].Substring(7);
            formatoCrecimiento = formatoCrecimiento.Replace(@"_", " ");
            var growthAmount = infoData["crecimiento"].Count;
            for (int i = 0; i < growthAmount; i++)
            {
                if (formatoCrecimiento == infoData["crecimiento"][i.ToString()])
                {
                    crecimiento.SelectedIndex = i;
                }
            }
            //crecimiento.SelectedIndex = Int32.Parse(infoData["crecimiento"].FirstOrDefault(x => x.Value.Contains(formatoCrecimiento)).Key);

            //a.Field<string>("Synonym(System name)").Split(',').Any( s => s == b.Field<string>("SystemName"))

            //Recibir huevo 1
            var formatoHuevo1 = pokemonData["grupoHuevoUno"][comboBox1.SelectedIndex.ToString()].Substring(10);
            formatoHuevo1 = formatoHuevo1.Replace(@"_", " ");
            HUEVO1.SelectedIndex = Int32.Parse(infoData["grupos_huevo"].FirstOrDefault(x => x.Value.Contains(formatoHuevo1)).Key);
            //Recibir huevo 2
            var formatoHuevo2 = pokemonData["grupoHuevoDos"][comboBox1.SelectedIndex.ToString()].Substring(10);
            formatoHuevo2 = formatoHuevo2.Replace(@"_", " ");
            HUEVO2.SelectedIndex = Int32.Parse(infoData["grupos_huevo"].FirstOrDefault(x => x.Value.Contains(formatoHuevo2)).Key);

            //Recibir habilidad 1
            var formatoHabilidad1 = pokemonData["habilidadUno"][comboBox1.SelectedIndex.ToString()].Substring(8);
            formatoHabilidad1 = formatoHabilidad1.Replace(@"_", " ");
            HABILIDAD1.SelectedIndex = Int32.Parse(infoData["habilidades"].FirstOrDefault(x => x.Value.Contains(formatoHabilidad1)).Key);
            //Recibir habilidad 2
            var formatoHabilidad2 = pokemonData["habilidadDos"][comboBox1.SelectedIndex.ToString()].Substring(8);
            formatoHabilidad2 = formatoHabilidad2.Replace(@"_", " ");
            HABILIDAD2.SelectedIndex = Int32.Parse(infoData["habilidades"].FirstOrDefault(x => x.Value.Contains(formatoHabilidad2)).Key);

            huidaSafari.Text = pokemonData["probabilidadHuidaSafari"][comboBox1.SelectedIndex.ToString()];

            //Recibir color del cuerpo
            var formatoColorCuerpo = pokemonData["colorCuerpo"][comboBox1.SelectedIndex.ToString()].Substring(11);
            formatoColorCuerpo = formatoColorCuerpo.Replace(@"_", " ");
            COLOR_CUERPO.SelectedIndex = Int32.Parse(infoData["color_cuerpo"].FirstOrDefault(x => x.Value.Contains(formatoColorCuerpo)).Key);

            POKEMON_NAME.Text = pokemonData["pokemonName"][comboBox1.SelectedIndex.ToString()];

            //EVOLUTION
            this.dataGridView2.Rows.Clear();
            var evosAmount = 0;
            if (evolutionData.ContainsKey(comboBox1.SelectedIndex.ToString() + "_0") == true)
            {
                evosAmount = Int32.Parse((evolutionData[comboBox1.SelectedIndex.ToString() + "_0"].Item1));
            }
            else
            {
                evosAmount = 0;
            }
            if (evosAmount > 0) {
                EvolucionAEliminar.Maximum = evosAmount - 1;
            } else
            {
                EvolucionAEliminar.Maximum = 0;
            }

            for (int i = 0; i < evosAmount; i++)
            {
                if (evolutionData.ContainsKey(comboBox1.SelectedIndex.ToString() + "_0") == true)
                {
                    var method = ((evolutionData[comboBox1.SelectedIndex.ToString() + "_" + i.ToString()].Item2).Replace(@"_", " ")).Substring(4);
                    var argument = (evolutionData[comboBox1.SelectedIndex.ToString() + "_" + i.ToString()].Item3).Replace(@" ", "");
                    var evolution = ((evolutionData[comboBox1.SelectedIndex.ToString() + "_" + i.ToString()].Item4).Replace(@"_", " ")).Substring(8);
                    this.dataGridView2.Rows.Add(method, argument, evolution);
                }
            }

            //MOVEMENTS

            this.dataGridView1.Rows.Clear();

            var moveAmount = moveData[comboBox1.SelectedIndex.ToString() + "_0"].Item1;
            for (int i = 0; i < Int32.Parse(moveAmount); i++)
            {
                var level = moveData[comboBox1.SelectedIndex.ToString() + "_" + i.ToString()].Item2;
                var movement = (moveData[comboBox1.SelectedIndex.ToString() + "_" + i.ToString()].Item3).Substring(5).Replace(@"_", " ");
                this.dataGridView1.Rows.Add(movement, level);
            }
            MovimientoAEliminar.Maximum = Int32.Parse(moveAmount) - 1;

            //MT/MO
            this.dataGridView3.Rows.Clear();
            var mtmoAmount = 0;

            if (mtmoData.ContainsKey(comboBox1.SelectedIndex.ToString() + "_0") == true) {
                mtmoAmount = Int32.Parse(mtmoData[comboBox1.SelectedIndex.ToString() + "_0"].Item1);
            } else
            {
                mtmoAmount = 0;
            }

            if (mtmoAmount > 0)
            {
                MTMOAEliminar.Maximum = mtmoAmount - 1;
            }
            else
            {
                MTMOAEliminar.Maximum = 0;
            }

            for (int i = 0; i < mtmoAmount; i++)
            {
                var mtmo = mtmoData[comboBox1.SelectedIndex.ToString() + "_" + i.ToString()].Item2.Replace(@"_", " ");
                this.dataGridView3.Rows.Add(mtmo);
            }



            //POKEDEX INFORMATION
            if (pokemonData["pokedexPageOne"].ContainsKey(comboBox1.SelectedIndex.ToString()))
            {
                descripcionUno.Text = pokemonData["pokedexPageOne"][comboBox1.SelectedIndex.ToString()];
            } else
            {
                descripcionUno.Text = "";
            }
            if (pokemonData["pokedexPageTwo"].ContainsKey(comboBox1.SelectedIndex.ToString()))
            {
                descripcionDos.Text = pokemonData["pokedexPageTwo"][comboBox1.SelectedIndex.ToString()];
            } else
            {
                descripcionDos.Text = "";
            }

            if (pokemonData["categoriaPokemon"].ContainsKey(comboBox1.SelectedIndex.ToString()))
            {
                categoriaPokemon.Text = pokemonData["categoriaPokemon"][comboBox1.SelectedIndex.ToString()];
            }
            else
            {
                categoriaPokemon.Text = "";
            }

            if (pokemonData["altura"].ContainsKey(comboBox1.SelectedIndex.ToString()))
            {
                altura.Text = pokemonData["altura"][comboBox1.SelectedIndex.ToString()];
            }
            else
            {
                altura.Text = "";
            }

            if (pokemonData["peso"].ContainsKey(comboBox1.SelectedIndex.ToString()))
            {
                peso.Text = pokemonData["peso"][comboBox1.SelectedIndex.ToString()];
            }
            else
            {
                peso.Text = "";
            }

            if (pokemonData["escalaPokemon"].ContainsKey(comboBox1.SelectedIndex.ToString()))
            {
                escalaPokemon.Text = pokemonData["escalaPokemon"][comboBox1.SelectedIndex.ToString()];
            }
            else
            {
                escalaPokemon.Text = "";
            }

            if (pokemonData["offsetPokemon"].ContainsKey(comboBox1.SelectedIndex.ToString()))
            {
                offsetPokemon.Text = pokemonData["offsetPokemon"][comboBox1.SelectedIndex.ToString()];
            }
            else
            {
                offsetPokemon.Text = "";
            }

            if (pokemonData["escalaEntrenador"].ContainsKey(comboBox1.SelectedIndex.ToString()))
            {
                escalaEntrenador.Text = pokemonData["escalaEntrenador"][comboBox1.SelectedIndex.ToString()];
            }
            else
            {
                escalaEntrenador.Text = "";
            }

            if (pokemonData["offsetEntrenador"].ContainsKey(comboBox1.SelectedIndex.ToString()))
            {
                offsetEntrenador.Text = pokemonData["offsetEntrenador"][comboBox1.SelectedIndex.ToString()];
            }
            else
            {
                offsetEntrenador.Text = "";
            }

            //SPRITES
            frontY.Value = Int32.Parse(pokemonData["frontCord"][comboBox1.SelectedIndex.ToString()]);
            backY.Value = Int32.Parse(pokemonData["backCord"][comboBox1.SelectedIndex.ToString()]);
            Levitation.Value = Int32.Parse(pokemonData["elevate"][comboBox1.SelectedIndex.ToString()]);
            iconPalette.SelectedIndex = Int32.Parse(pokemonData["palUsed"][comboBox1.SelectedIndex.ToString()]);
            var formatPokemonName = (comboBox1.Text).ToString().ToLower().Replace(" ", "_");

            bool backExist = File.Exists(dictionary["pFolder_pokemon"] + "\\" + formatPokemonName + "\\back.png");
            bool frontExist = File.Exists(dictionary["pFolder_pokemon"] + "\\" + formatPokemonName + "\\front.png");
            bool footprintExist = File.Exists(dictionary["pFolder_pokemon"] + "\\" + formatPokemonName + "\\footprint.png");
            bool iconExist = File.Exists(dictionary["pFolder_pokemon"] + "\\" + formatPokemonName + "\\icon.png");

            if (backExist) {
                pictureBox2.Image = Image.FromFile(dictionary["pFolder_pokemon"] + "\\" + formatPokemonName + "\\back.png");
            } else
            {
                pictureBox2.Image = null;
            }
            if (frontExist) {
                pictureBox3.Image = Image.FromFile(dictionary["pFolder_pokemon"] + "\\" + formatPokemonName + "\\front.png");
            }
            else
            {
                pictureBox3.Image = null;
            }
            if (footprintExist)
            {
                footprint.Image = Image.FromFile(dictionary["pFolder_pokemon"] + "\\" + formatPokemonName + "\\footprint.png");
            }
            else
            {
                footprint.Image = null;
            }
            if (iconExist)
            {
                icon.Image = Image.FromFile(dictionary["pFolder_pokemon"] + "\\" + formatPokemonName + "\\icon.png");
            }
            else
            {
                icon.Image = null;
            }
            if (backExist && frontExist)
            {
                setSpritePosition();
            }



            /*pokemonData.Add("categoriaPokemon", new Dictionary<string, string>());
            pokemonData.Add("altura", new Dictionary<string, string>());
            pokemonData.Add("peso", new Dictionary<string, string>());
            pokemonData.Add("escalaPokemon", new Dictionary<string, string>());
            pokemonData.Add("offsetPokemon", new Dictionary<string, string>());
            pokemonData.Add("escalaEntrenador", new Dictionary<string, string>());
            pokemonData.Add("offsetEntrenador", new Dictionary<string, string>());*/


        }

        //SET FUNCTIONS

        private void setBackPicTable()
        {
            //obj_tiles gMonBackPic_Chimecho, 0x800, SPECIES_CHIMECHO

            string str = null;

            StreamReader sr = new StreamReader(dictionary["pFile_back_pic_table_inc"].ToString());
            str = sr.ReadToEnd();
            sr.Close();

            var index = str.IndexOf("obj_tiles gMonPic_Egg, 0x800, SPECIES_EGG");
            var preStr = str.Substring(0, index);
            var postStr = str.Substring(index);
            str = preStr + "obj_tiles gMonBackPic_" + System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(POKEMON_NAME.Text.ToLower()) + ", 0x800, SPECIES_" + POKEMON_NAME.Text.ToUpper() + Environment.NewLine + "\t" + postStr;
            data["pFile_back_pic_table_inc"] = str;

            StreamWriter sw = new StreamWriter(dictionary["pFile_back_pic_table_inc"].ToString(), false);
            sw.WriteLine(data["pFile_back_pic_table_inc"]);
            sw.Close();
        }

        private void setPokemonDataDictionary()
        {
            pokemonData.Add("pokemonName", new Dictionary<string, string>());
            pokemonData.Add("psBase", new Dictionary<string, string>());
            pokemonData.Add("ataqueBase", new Dictionary<string, string>());
            pokemonData.Add("defensaBase", new Dictionary<string, string>());
            pokemonData.Add("velocidadBase", new Dictionary<string, string>());
            pokemonData.Add("ataqueEspecialBase", new Dictionary<string, string>());
            pokemonData.Add("defensaEspecialBase", new Dictionary<string, string>());
            pokemonData.Add("tipoUno", new Dictionary<string, string>());
            pokemonData.Add("tipoDos", new Dictionary<string, string>());
            pokemonData.Add("ratioDeCaptura", new Dictionary<string, string>());
            pokemonData.Add("expBase", new Dictionary<string, string>());
            pokemonData.Add("evsPS", new Dictionary<string, string>());
            pokemonData.Add("evsAtaque", new Dictionary<string, string>());
            pokemonData.Add("evsDefensa", new Dictionary<string, string>());
            pokemonData.Add("evsVelocidad", new Dictionary<string, string>());
            pokemonData.Add("evsAtaqueEspecial", new Dictionary<string, string>());
            pokemonData.Add("evsDefensaEspecial", new Dictionary<string, string>());
            pokemonData.Add("objetoUno", new Dictionary<string, string>());
            pokemonData.Add("objetoDos", new Dictionary<string, string>());
            pokemonData.Add("ratioGenero", new Dictionary<string, string>());
            pokemonData.Add("tieneGenero", new Dictionary<string, string>());
            pokemonData.Add("ciclosHuevo", new Dictionary<string, string>());
            pokemonData.Add("amistadBase", new Dictionary<string, string>());
            pokemonData.Add("crecimiento", new Dictionary<string, string>());
            pokemonData.Add("grupoHuevoUno", new Dictionary<string, string>());
            pokemonData.Add("grupoHuevoDos", new Dictionary<string, string>());
            pokemonData.Add("habilidadUno", new Dictionary<string, string>());
            pokemonData.Add("habilidadDos", new Dictionary<string, string>());
            pokemonData.Add("probabilidadHuidaSafari", new Dictionary<string, string>());
            pokemonData.Add("colorCuerpo", new Dictionary<string, string>());
            pokemonData.Add("pokedexPageOne", new Dictionary<string, string>());
            pokemonData.Add("pokedexPageTwo", new Dictionary<string, string>());
            pokemonData.Add("categoriaPokemon", new Dictionary<string, string>());
            pokemonData.Add("altura", new Dictionary<string, string>());
            pokemonData.Add("peso", new Dictionary<string, string>());
            pokemonData.Add("escalaPokemon", new Dictionary<string, string>());
            pokemonData.Add("offsetPokemon", new Dictionary<string, string>());
            pokemonData.Add("escalaEntrenador", new Dictionary<string, string>());
            pokemonData.Add("offsetEntrenador", new Dictionary<string, string>());
            pokemonData.Add("backCord", new Dictionary<string, string>());
            pokemonData.Add("frontCord", new Dictionary<string, string>());
            pokemonData.Add("elevate", new Dictionary<string, string>());
            pokemonData.Add("palUsed", new Dictionary<string, string>());
        }

        private void cleanAll()
        {
            dictionary = new Dictionary<string, string>();
            data = new Dictionary<string, string>();
            pokemonData = new Dictionary<string, Dictionary<string, string>>();
            infoData = new Dictionary<string, Dictionary<string, string>>();
            evolutionData = new Dictionary<string, Tuple<string, string, string, string>>();
            moveData = new Dictionary<string, Tuple<string, string, string>>();
            mtmoData = new Dictionary<string, Tuple<string, string>>();

            directory = null;
            mayusPokemonName = null;
            minusPokemonName = null;
            firstMayusPokemonName = null;
            fileName = null;
            pathErr = null;
            fileErr = null;

            comboBox1.Items.Clear();
            comboBox2.Items.Clear();
            comboBox3.Items.Clear();
            TIPO1.Items.Clear();
            TIPO2.Items.Clear();
            HUEVO1.Items.Clear();
            HUEVO2.Items.Clear();
            HABILIDAD1.Items.Clear();
            HABILIDAD2.Items.Clear();
            OBJETO1.Items.Clear();
            OBJETO2.Items.Clear();
            iconPalette.Items.Clear();

            dataGridView1.Rows.Clear();
            dataGridView2.Rows.Clear();
            dataGridView3.Rows.Clear();

        }

        private void setDataDictionary()
        {
            string str = null;

            string[] filePaths = {
                ("pFile_back_pic_table_inc"),
                ("pFile_front_pic_table_inc"),
                ("pFile_back_pic_coords_inc"),
                ("pFile_front_pic_coords_inc"),
                ("pFile_graphics_inc"),
                ("pFile_palette_table_inc"),
                ("pFile_shiny_palette_table_inc"),
                ("pFile_species_h"),
                ("pFile_global_h"),
                ("pFile_graphics_h"),
                ("pFile_pokedex_h"),
                ("pFile_direct_sound_data_inc"),
                ("pFile_voice_groups_inc"),
                ("pFile_battle_1_c"),
                ("pFile_base_stats_h"),
                ("pFile_cry_ids_h"),
                ("pFile_level_up_learnset_pointers_h"),
                ("pFile_level_up_learnsets_h"),
                ("pFile_tmhm_learnsets_h"),
                ("pFile_species_names_en_h"),
                ("pFile_pokedex_entries_en_h"),
                ("pFile_pokedex_orders_h"),
                ("pFile_pokedex_c"),
                ("pFile_pokemon_1_c"),
                ("pFile_pokemon_icon_c"),
                ("pFile_move_names_en_h"),
                ("pFile_items_en_h"),
                ("pFile_pokemon_h"),
                ("pFile_abilities_h"),
                ("pFile_evolution_h")
            };

            for (int i = 0; i < filePaths.Length; i++)
            {
                StreamReader sr = new StreamReader(dictionary[filePaths[i].ToString()].ToString());
                str = sr.ReadToEnd();
                data[filePaths[i].ToString()] = str;
                sr.Close();
            }

            //richTextBox1.Text = data["pFile_base_stats_h"];
        }

        private void setInfoDataDictionary()
        {
            infoData.Add("tipos", new Dictionary<string, string>());
            infoData.Add("movimientos", new Dictionary<string, string>());
            infoData.Add("mt", new Dictionary<string, string>());
            infoData.Add("mo", new Dictionary<string, string>());
            infoData.Add("objetos", new Dictionary<string, string>());
            infoData.Add("habilidades", new Dictionary<string, string>());
            infoData.Add("grupos_huevo", new Dictionary<string, string>());
            infoData.Add("color_cuerpo", new Dictionary<string, string>());
            infoData.Add("crecimiento", new Dictionary<string, string>());
            infoData.Add("metodoEvolutivo", new Dictionary<string, string>());

            setTypesData();
            setMovementsData();
            setItemsData();
            setMTMOData();
            setEggAndColorData();
            setAbilitiesData();
            setGrowthData();
            setEvolutiveMethodData();
        }

        private void setTypesData()
        {
            string str = data["pFile_pokemon_h"].ToString();
            int index = 0;
            int lastIndex = 0;

            var totalTypes = Regex.Matches(str, "TYPE_").Cast<Match>().Count() - 1;

            for (int i = 0; i <= totalTypes; i++)
            {
                index = str.IndexOf("TYPE_", lastIndex + 2);
                lastIndex = index;

                var typeName = str.Substring((index + 5), ((str.IndexOf(" ", index)) - (index + 5)));

                infoData["tipos"][i.ToString()] = typeName;
                //MessageBox.Show("Tipo: " + infoData["tipos"][i.ToString()]);
            }

            addTypesToComboBox();
        }

        private void setMovementsData()
        {
            string str = data["pFile_move_names_en_h"].ToString();
            int index = 0;
            int lastIndex = 0;

            var totalMoves = Regex.Matches(str, "MOVE_").Cast<Match>().Count() - 1;

            for (int i = 0; i <= totalMoves; i++)
            {
                index = str.IndexOf("MOVE_", lastIndex + 2);
                lastIndex = index;

                var typeName = str.Substring((str.IndexOf("_", index) + 1), (str.IndexOf("=", index) - 2) - (str.IndexOf("_", index) + 1));

                infoData["movimientos"][i.ToString()] = typeName;
                //MessageBox.Show("Ataque: " + infoData["movimientos"][i.ToString()]);
            }
        }

        private void setItemsData()
        {
            string str = data["pFile_items_en_h"].ToString();
            int index = 0;
            int lastIndex = 0;

            var totalItems = Regex.Matches(str, ".name =").Cast<Match>().Count() - 1;
            var countMT = Regex.Matches(str, "ITEM_TM").Cast<Match>().Count();
            var countMO = Regex.Matches(str, "ITEM_HM").Cast<Match>().Count();

            totalItems = totalItems - (countMT + countMO);

            for (int i = 0; i <= totalItems; i++)
            {
                index = str.IndexOf(".itemId", lastIndex + 2);
                lastIndex = index;

                var item = str.Substring((index + 15), ((str.IndexOf(",", index)) - (index + 15)));

                item = item.Replace(@"_", " ");

                infoData["objetos"][i.ToString()] = item;
                //MessageBox.Show("Item: " + infoData["objetos"][i.ToString()]);
            }

            addItemsToComboBox();
        }

        private void setMTMOData()
        {
            string str = data["pFile_items_en_h"].ToString();
            int index = 0;
            int lastIndex = 0;

            var countMT = Regex.Matches(str, "ITEM_TM").Cast<Match>().Count() - 1;
            var countMO = Regex.Matches(str, "ITEM_HM").Cast<Match>().Count() - 1;

            for (int i = 0; i <= countMT; i++)
            {
                index = str.IndexOf(".itemId = ITEM_TM", lastIndex + 2);
                lastIndex = index;

                var mt = str.Substring((index + 15), ((str.IndexOf(",", index)) - (index + 15)));

                mt = mt.Replace(@"_", " ");

                infoData["mt"][i.ToString()] = mt;
                //MessageBox.Show("Item: " + infoData["mt"][i.ToString()]);
            }

            index = 0;
            lastIndex = 0;

            for (int i = 0; i <= countMO; i++)
            {
                index = str.IndexOf(".itemId = ITEM_HM", lastIndex + 2);
                lastIndex = index;

                var mo = str.Substring((index + 15), ((str.IndexOf(",", index)) - (index + 15)));

                mo = mo.Replace(@"_", " ");

                infoData["mo"][i.ToString()] = mo;
                //MessageBox.Show("Item: " + infoData["mo"][i.ToString()]);
            }
        }

        private void setEggAndColorData()
        {
            string str = data["pFile_pokemon_h"].ToString();
            int index = 0;
            int lastIndex = 0;

            var EggGroup = Regex.Matches(str, "EGG_GROUP_").Cast<Match>().Count() - 1;
            var BodyColor = Regex.Matches(str, "BODY_COLOR_").Cast<Match>().Count() - 1;

            for (int i = 0; i <= EggGroup; i++)
            {
                index = str.IndexOf("EGG_GROUP_", lastIndex + 2);
                lastIndex = index;

                var eggGroup = str.Substring((index + 10), (str.IndexOf(",", index) - (index + 10)));
                if (eggGroup.Length > 12)
                {
                    eggGroup = eggGroup.Substring(0, (eggGroup.IndexOf("}", 0) - 1));
                }

                eggGroup = eggGroup.Replace(@"_", " ");

                infoData["grupos_huevo"][i.ToString()] = eggGroup;
                //MessageBox.Show("Grupos huevo: " + infoData["grupos_huevo"][i.ToString()]);
            }

            index = 0;
            lastIndex = 0;

            for (int i = 0; i <= BodyColor; i++)
            {
                index = str.IndexOf("BODY_COLOR_", lastIndex + 2);
                lastIndex = index;

                var bodyColor = str.Substring((index + 11), (str.IndexOf(",", index) - (index + 11)));
                if (bodyColor.Length > 12)
                {
                    bodyColor = bodyColor.Substring(0, (bodyColor.IndexOf("}", 0) - 1));
                }

                bodyColor = bodyColor.Replace(@"_", " ");

                infoData["color_cuerpo"][i.ToString()] = bodyColor;
                //MessageBox.Show("Color cuerpo: " + infoData["color_cuerpo"][i.ToString()]);
            }

            addEggGroupToComboBox();
            addBodyColorToComboBox();
        }

        private void setAbilitiesData()
        {
            string str = data["pFile_abilities_h"].ToString();
            int index = 0;
            int lastIndex = 0;

            var totalAbilities = Regex.Matches(str, "ABILITY_").Cast<Match>().Count() - 1;

            for (int i = 0; i <= totalAbilities; i++)
            {
                index = str.IndexOf("ABILITY_", lastIndex + 2);
                lastIndex = index;

                var typeName = str.Substring((index + 8), (str.IndexOf(" ", index)) - (index + 8));

                typeName = typeName.Replace(@"_", " ");

                infoData["habilidades"][i.ToString()] = typeName;
                //MessageBox.Show("Habilidades: " + infoData["habilidades"][i.ToString()]);
            }

            addAbilitiesToComboBox();
        }

        private void setGrowthData()
        {
            string str = data["pFile_pokemon_h"].ToString();
            int index = 0;
            int lastIndex = 0;

            var totalGrowthModes = Regex.Matches(str, "GROWTH_").Cast<Match>().Count() - 1;

            for (int i = 0; i <= totalGrowthModes; i++)
            {
                index = str.IndexOf("GROWTH_", lastIndex + 2);
                lastIndex = index;

                var growthName = str.Substring((index + 7), (str.IndexOf(",", index) - (index + 7)));
                if (growthName.Length > 12)
                {
                    growthName = growthName.Substring(0, (growthName.IndexOf("}", 0) - 1));
                }

                growthName = growthName.Replace(@"_", " ");

                infoData["crecimiento"][i.ToString()] = growthName;
                //MessageBox.Show("Crecimiento: " + infoData["crecimiento"][i.ToString()]);
            }

            addGrowthToComboBox();
        }

        private void setEvolutiveMethodData()
        {
            string str = data["pFile_pokemon_h"].ToString();
            int index = 0;
            int lastIndex = 0;

            var totalEvolutiveMethods = Regex.Matches(str, "EVO_").Cast<Match>().Count() - 1;

            for (int i = 0; i <= totalEvolutiveMethods; i++)
            {
                index = str.IndexOf("EVO_", lastIndex + 2);
                lastIndex = index;

                var evoMethodName = str.Substring((index + 4), (str.IndexOf(" ", index) - (index + 4)));

                evoMethodName = evoMethodName.Replace(@"_", " ");

                infoData["metodoEvolutivo"][i.ToString()] = evoMethodName;
                //MessageBox.Show("Método evolutivo: " + infoData["metodoEvolutivo"][i.ToString()]);
            }

            addEvoToComboBox();
        }

        //ADD FUNCTIONS

        private void addTypesToComboBox()
        {
            int typesAmount = infoData["tipos"].Count;

            TIPO1.Items.Clear();
            TIPO2.Items.Clear();

            for (int i = 0; i < typesAmount; i++)
            {
                string insertTypeName = infoData["tipos"][i.ToString()];
                //MessageBox.Show(insertTypeName);
                TIPO1.Items.Insert(i, insertTypeName);
                TIPO2.Items.Insert(i, insertTypeName);
            }
        }

        private void addItemsToComboBox()
        {
            int objectAmount = infoData["objetos"].Count;

            OBJETO1.Items.Clear();
            OBJETO2.Items.Clear();

            for (int i = 0; i < objectAmount; i++)
            {
                string insertObjectName = infoData["objetos"][i.ToString()];
                //MessageBox.Show(insertObjectName);
                OBJETO1.Items.Insert(i, insertObjectName);
                OBJETO2.Items.Insert(i, insertObjectName);
            }
        }

        private void addGrowthToComboBox()
        {
            int growthAmount = infoData["crecimiento"].Count;

            crecimiento.Items.Clear();

            for (int i = 0; i < growthAmount; i++)
            {
                string insertGrowthName = infoData["crecimiento"][i.ToString()];
                //MessageBox.Show(insertGrowthName);
                crecimiento.Items.Insert(i, insertGrowthName);
            }
        }

        private void addEggGroupToComboBox()
        {
            int eggGroupAmount = infoData["grupos_huevo"].Count;

            HUEVO1.Items.Clear();
            HUEVO2.Items.Clear();

            for (int i = 0; i < eggGroupAmount; i++)
            {
                string insertEggGroupName = infoData["grupos_huevo"][i.ToString()];
                //MessageBox.Show(insertEggGroupName);
                HUEVO1.Items.Insert(i, insertEggGroupName);
                HUEVO2.Items.Insert(i, insertEggGroupName);
            }
        }

        private void addBodyColorToComboBox()
        {
            int bodyColorAmount = infoData["color_cuerpo"].Count;

            COLOR_CUERPO.Items.Clear();

            for (int i = 0; i < bodyColorAmount; i++)
            {
                string insertBodyColorName = infoData["color_cuerpo"][i.ToString()];
                //MessageBox.Show(insertBodyColorName);
                COLOR_CUERPO.Items.Insert(i, insertBodyColorName);
            }
        }

        private void addAbilitiesToComboBox()
        {
            int abilitiesAmount = infoData["habilidades"].Count;

            HABILIDAD1.Items.Clear();
            HABILIDAD2.Items.Clear();

            for (int i = 0; i < abilitiesAmount; i++)
            {
                string insertAbilityName = infoData["habilidades"][i.ToString()];
                //MessageBox.Show(insertAbilityName);
                HABILIDAD1.Items.Insert(i, insertAbilityName);
                HABILIDAD2.Items.Insert(i, insertAbilityName);
            }
        }

        private void addEvoToComboBox()
        {
            int evoMethodsAmount = infoData["metodoEvolutivo"].Count;

            Metodo.Items.Clear();

            for (int i = 0; i < evoMethodsAmount; i++)
            {
                string insertEvoMethodName = infoData["metodoEvolutivo"][i.ToString()];
                //MessageBox.Show(insertEvoMethodName);
                Metodo.Items.Insert(i, insertEvoMethodName);
            }
        }

        /*
        private void addArgumentsData()
        {
            int objectAmount = infoData["objetos"].Count;

            Argumento.Items.Clear();

            for (int i = 0; i < 254 + objectAmount; i++)
            {
                if (i <= 255)
                {
                    Argumento.Items.Insert(i, i.ToString());
                } else
                {
                    string insertObjectName = infoData["objetos"][(i - 256).ToString()];
                    Argumento.Items.Insert(i, insertObjectName);
                }

            }
        }*/

        private void addMovementsData()
        {
            int movementAmount = infoData["movimientos"].Count;

            Ataque.Items.Clear();
            Nivel.Items.Clear();

            for (int i = 0; i <= 100; i++)
            {
                Nivel.Items.Insert(i, i.ToString());
            }

            for (int i = 0; i < movementAmount; i++)
            {
                var movement = (infoData["movimientos"][i.ToString()]).Replace(@"_", " ");
                Ataque.Items.Insert(i, movement);
            }
        }

        private void addMTMOData()
        {
            int mtAmount = infoData["mt"].Count;
            int moAmount = infoData["mo"].Count;

            MTName.Items.Clear();

            for (int i = 0; i < (mtAmount + moAmount); i++)
            {
                if (i < mtAmount) {
                    var movement = (infoData["mt"][i.ToString()]);
                    MTName.Items.Insert(i, movement);
                } else
                {
                    var movement = (infoData["mo"][(i - mtAmount).ToString()]);
                    MTName.Items.Insert(i, movement);
                }
            }
        }

        //SAVE FUNCTIONS

        private void saveData()
        {
            setBaseStats();
            setEvolutions();
            setMovements();
            setMTMO();
            setDexDescription();
            setDexData();
            setSpriteData();
            reloadAllSavedData();
        }

        private void setBaseStats()
        {
            var pokemonSpecie = comboBox1.Text;
            var baseHP = PS_Base.Text;
            var baseAttack = ATQ_Base.Text;
            var baseDefense = DEF_Base.Text;
            var baseSpeed = VEL_Base.Text;
            var baseSpAttack = ATESP_Base.Text;
            var baseSpDefense = DFESP_Base.Text;
            var type1 = (TIPO1.Text).Replace(" ", "_");
            var type2 = (TIPO2.Text).Replace(" ", "_");
            var catchRate = ratioCaptura.Text;
            var expYield = expBase.Text;
            var evYield_HP = PS_Effort.Text;
            var evYield_Attack = ATQ_Effort.Text;
            var evYield_Defense = DEF_Effort.Text;
            var evYield_Speed = VEL_Effort.Text;
            var evYield_SpAttack = ATESP_Effort.Text;
            var evYield_SpDefense = DFESP_Effort.Text;
            var item1 = (OBJETO1.Text).Replace(" ", "_");
            var item2 = (OBJETO2.Text).Replace(" ", "_");
            var genderRatio = "";
            if (generoCheck.Checked == true)
            {
                if (genero.Text == "0")
                {
                    genderRatio = "MON_MALE";
                } else if (genero.Text == "100")
                {
                    genderRatio = "MON_FEMALE";
                } else
                {
                    genderRatio = "PERCENT_FEMALE(" + genero.Text + ")";
                }

            } else
            {
                genderRatio = "MON_GENDERLESS";
            }
            var eggCycles = ciclosHuevo.Text;
            var friendship = amistadBase.Text;
            var growthRate = (crecimiento.Text).Replace(" ", "_");
            var eggGroup1 = (HUEVO1.Text).Replace(" ", "_");
            var eggGroup2 = (HUEVO2.Text).Replace(" ", "_");
            var ability1 = (HABILIDAD1.Text).Replace(" ", "_");
            var ability2 = (HABILIDAD2.Text).Replace(" ", "_");
            var safariZoneFleeRate = huidaSafari.Text;
            var bodyColor = (COLOR_CUERPO.Text).Replace(" ", "_");

            string finalString = "    [SPECIES_" + pokemonSpecie.Replace(" ", "_") + "] =\n    {\n        .baseHP        = " + baseHP +
                ",\n        .baseAttack    = " + baseAttack +
                ",\n        .baseDefense   = " + baseDefense +
                ",\n        .baseSpeed     = " + baseSpeed +
                ",\n        .baseSpAttack  = " + baseSpAttack +
                ",\n        .baseSpDefense = " + baseSpDefense +
                ",\n        .type1 = TYPE_" + type1 +
                ",\n        .type2 = TYPE_" + type2 +
                ",\n        .catchRate = " + catchRate +
                ",\n        .expYield = " + expYield +
                ",\n        .evYield_HP        = " + evYield_HP +
                ",\n        .evYield_Attack    = " + evYield_Attack +
                ",\n        .evYield_Defense   = " + evYield_Defense +
                ",\n        .evYield_Speed     = " + evYield_Speed +
                ",\n        .evYield_SpAttack  = " + evYield_SpAttack +
                ",\n        .evYield_SpDefense = " + evYield_SpDefense +
                ",\n        .item1 = ITEM_" + item1 +
                ",\n        .item2 = ITEM_" + item2 +
                ",\n        .genderRatio = " + genderRatio +
                ",\n        .eggCycles = " + eggCycles +
                ",\n        .friendship = " + friendship +
                ",\n        .growthRate = GROWTH_" + growthRate +
                ",\n        .eggGroup1 = EGG_GROUP_" + eggGroup1 +
                ",\n        .eggGroup2 = EGG_GROUP_" + eggGroup2 +
                ",\n        .ability1 = ABILITY_" + ability1 +
                ",\n        .ability2 = ABILITY_" + ability2 +
                ",\n        .safariZoneFleeRate = " + safariZoneFleeRate +
                ",\n        .bodyColor = BODY_COLOR_" + bodyColor +
                ",\n        .noFlip = FALSE,\n    },";

            string str = null;

            StreamReader sr = new StreamReader(dictionary["pFile_base_stats_h"].ToString());
            str = sr.ReadToEnd();
            sr.Close();

            var index = str.IndexOf("[SPECIES_" + pokemonSpecie.Replace(" ", "_")) - 4;
            var index2 = str.IndexOf("}", index) + 1;
            var preStr = str.Substring(0, index);
            var postStr = str.Substring(index2 + 1);
            str = preStr + finalString + postStr;
            index = str.LastIndexOf("[SPECIES");
            index = str.IndexOf("},", index);
            if (index > 0)
            {
                preStr = str.Substring(0, index);
                postStr = str.Substring(index + 2);
                str = preStr + "}\n" + postStr;
            }
            data["pFile_base_stats_h"] = str;

            //richTextBox1.Text = str;

            StreamWriter sw = new StreamWriter(dictionary["pFile_base_stats_h"].ToString(), false);
            sw.WriteLine(data["pFile_base_stats_h"]);
            sw.Close();

            //MessageBox.Show(pokemonName);
        }

        private void setEvolutions()
        {
            //var evolutions = dataGridView2.Rows[0].Cells[2].Value.ToString();
            var totalEvolutions = dataGridView2.Rows.Count - 1;
            var pokemonSpecie = comboBox1.Text;

            string[] metodo = new string[totalEvolutions];
            string[] argumento = new string[totalEvolutions];
            string[] evolucion = new string[totalEvolutions];
            string finalString = null;

            if (totalEvolutions > 0) {

                var spaceValue = 11 - pokemonSpecie.Length;

                finalString = "    [SPECIES_" + pokemonSpecie.Replace(" ", "_") + "]";

                for (int i = 0; i < totalEvolutions; i++)
                {
                    metodo[i] = dataGridView2.Rows[i].Cells[0].Value.ToString().Replace(" ", "_");
                    argumento[i] = dataGridView2.Rows[i].Cells[1].Value.ToString();
                    evolucion[i] = dataGridView2.Rows[i].Cells[2].Value.ToString().Replace(" ", "_");
                }

                for (int i = 0; i < spaceValue; i++)
                {
                    finalString += " ";
                }

                finalString += "= {";

                for (int i = 0; i < totalEvolutions; i++)
                {
                    finalString += "{EVO_" + metodo[i] + ", " + argumento[i] + ", SPECIES_" + evolucion[i] + "}";
                    if (i == (totalEvolutions - 1))
                    {
                        finalString += "},";
                    } else
                    {
                        finalString += ",\n                            ";
                    }
                }

                //richTextBox1.Text = finalString;

            } else
            {
                finalString = "";
            }


            string str = null;

            StreamReader sr = new StreamReader(dictionary["pFile_evolution_h"].ToString());
            str = sr.ReadToEnd();
            sr.Close();

            var index = str.IndexOf("[SPECIES_" + pokemonSpecie.Replace(" ", "_"));
            var index2 = 0;
            var preStr = "";
            var postStr = "";

            if (index >= 0)
            {
                if (totalEvolutions > 0)
                {
                    index = index - 4;
                    index2 = str.IndexOf("}},", index) + 2;
                    preStr = str.Substring(0, index);
                    postStr = str.Substring(index2 + 1);
                    str = preStr + finalString + postStr;
                } else if (totalEvolutions == 0)
                {
                    index = index - 4;
                    index2 = str.IndexOf("}},", index) + 2;
                    preStr = str.Substring(0, index);
                    postStr = str.Substring(index2 + 1);
                    str = preStr + postStr;
                }
            } else
            {
                if (totalEvolutions > 0)
                {
                    index = 0;
                    index2 = str.IndexOf("};", 0) - 1;
                    preStr = str.Substring(0, index2);
                    postStr = str.Substring(index2);
                    str = preStr + "\n" + finalString + "\n" + postStr;
                }
            }

            data["pFile_evolution_h"] = str;

            //richTextBox1.Text = str;

            StreamWriter sw = new StreamWriter(dictionary["pFile_evolution_h"].ToString(), false);
            sw.WriteLine(data["pFile_evolution_h"]);
            sw.Close();

        }

        private void setMovements()
        {
            var totalMovements = dataGridView1.Rows.Count - 1;
            var pokemonSpecie = comboBox1.Text;
            var formatPokemonName = (System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(pokemonSpecie.ToLower())).Replace(" ", "");
            if (formatPokemonName == "MrMime")
            {
                formatPokemonName = "Mrmime";
            }

            string[] ataque = new string[totalMovements];
            string[] nivel = new string[totalMovements];
            string finalString = null;

            if (totalMovements > 0)
            {
                for (int i = 0; i < totalMovements; i++)
                {
                    ataque[i] = dataGridView1.Rows[i].Cells[0].Value.ToString().Replace(" ", "_");
                    nivel[i] = dataGridView1.Rows[i].Cells[1].Value.ToString().Replace(" ", "_");
                }

                finalString = "const u16 g" + formatPokemonName + "LevelUpLearnset[] = {\n";
                for (int i = 0; i < totalMovements; i++)
                {
                    if (nivel[i].Length == 1)
                    {
                        nivel[i] = " " + nivel[i];
                    }
                    finalString += "    LEVEL_UP_MOVE(" + nivel[i] + ", MOVE_" + ataque[i] + "),\n";
                    //    LEVEL_UP_MOVE( 1, MOVE_TACKLE),
                }
                finalString += "    LEVEL_UP_END\n};";
            }
            else
            {
                finalString = "";
            }

            //richTextBox1.Text = finalString;

            string str = null;

            StreamReader sr = new StreamReader(dictionary["pFile_level_up_learnsets_h"].ToString());
            str = sr.ReadToEnd();
            sr.Close();

            var index = str.IndexOf("const u16 g" + formatPokemonName, 0);
            var index2 = str.IndexOf("};", index) + 1;
            var preStr = str.Substring(0, index);
            var postStr = str.Substring(index2 + 1);
            str = preStr + finalString + postStr;
            data["pFile_level_up_learnsets_h"] = str;

            //richTextBox1.Text = str;

            StreamWriter sw = new StreamWriter(dictionary["pFile_level_up_learnsets_h"].ToString(), false);
            sw.WriteLine(data["pFile_level_up_learnsets_h"]);
            sw.Close();
        }

        private void setMTMO()
        {
            var totalMTMO = dataGridView3.Rows.Count - 1;
            var pokemonSpecie = (comboBox1.Text).Replace(" ", "_");

            string[] MTMO = new string[totalMTMO];
            string finalString = null;

            if (totalMTMO > 0)
            {
                finalString = "    [SPECIES_" + pokemonSpecie + "]";
                var totalSpaces = 12 - pokemonSpecie.Length;

                for (int i = 0; i < totalSpaces; i++)
                {
                    finalString += " ";
                }

                finalString += "= TMHM_LEARNSET(TMHM(";

                for (int i = 0; i < totalMTMO; i++)
                {
                    MTMO[i] = dataGridView3.Rows[i].Cells[0].Value.ToString().Replace(" ", "_");
                    if (i == (totalMTMO - 1)) {
                        finalString += MTMO[i] + ")),";
                    } else
                    {
                        finalString += MTMO[i] + ")\n                                        | TMHM(";
                    }
                }
            }
            else
            {

                //[SPECIES_CATERPIE]    = TMHM_LEARNSET(0),
                finalString = "    [SPECIES_" + pokemonSpecie + "]";
                var totalSpaces = 12 - pokemonSpecie.Length;

                for (int i = 0; i < totalSpaces; i++)
                {
                    finalString += " ";
                }
                finalString += "= TMHM_LEARNSET(0),";
            }

            //richTextBox1.Text = finalString;

            string str = null;

            StreamReader sr = new StreamReader(dictionary["pFile_tmhm_learnsets_h"].ToString());
            str = sr.ReadToEnd();
            sr.Close();

            var index = str.IndexOf("    [SPECIES_" + pokemonSpecie, 0);
            var index2 = str.IndexOf("),", index) + 3;
            var preStr = str.Substring(0, index);
            var postStr = str.Substring(index2);
            str = preStr + finalString + "\n" + postStr;
            data["pFile_tmhm_learnsets_h"] = str;

            //richTextBox1.Text = str;

            StreamWriter sw = new StreamWriter(dictionary["pFile_tmhm_learnsets_h"].ToString(), false);
            sw.WriteLine(data["pFile_tmhm_learnsets_h"]);
            sw.Close();
        }

        private void setDexDescription()
        {
            //PAGE 1
            var dexDescriptionOne = (descripcionUno.Text);
            var dexFormatOne = "";
            var spacesAmountOne = dexDescriptionOne.Count(f => f == ' ');
            int[] spaceIndexOne = new int[spacesAmountOne];

            //PAGE 2
            var dexDescriptionTwo = (descripcionDos.Text);
            var dexFormatTwo = "";
            var spacesAmountTwo = dexDescriptionTwo.Count(f => f == ' ');
            int[] spaceIndexTwo = new int[spacesAmountTwo];

            var lastSpace = 0;
            int descriptionIndex = 0;
            int maxLengthPerLine = 39;
            int reducedAmount = 0;
            int rowCounter = 0;
            var pokemonName = "";
            var finalString = "";



            for (int i = 0; i < spacesAmountOne; i++)
            {
                spaceIndexOne[i] = dexDescriptionOne.IndexOf(" ", descriptionIndex);
                descriptionIndex = dexDescriptionOne.IndexOf(" ", descriptionIndex) + 1;
            }

            descriptionIndex = 0;

            for (int i = 0; i < spacesAmountTwo; i++)
            {
                spaceIndexTwo[i] = dexDescriptionTwo.IndexOf(" ", descriptionIndex);
                descriptionIndex = dexDescriptionTwo.IndexOf(" ", descriptionIndex) + 1;
            }

            for (int i = 0; i < spacesAmountOne; i++)
            {
                //MessageBox.Show((spaceIndex[i] - reducedAmount) + "\n" + maxLengthPerLine);
                if ((spaceIndexOne[i] - reducedAmount) > maxLengthPerLine)
                {
                    dexFormatOne += "  \"" + dexDescriptionOne.Substring(reducedAmount, lastSpace - reducedAmount) + "\\n\"\n";
                    reducedAmount += lastSpace - reducedAmount + 1;
                    rowCounter++;
                }
                if (i == spacesAmountOne - 1)
                {
                    dexFormatOne += "  \"" + dexDescriptionOne.Substring(reducedAmount) + "\"";
                }
                lastSpace = (spaceIndexOne[i]);
            }

            //MessageBox.Show(dexFormatOne + "\n" + rowCounter.ToString());


            if (rowCounter <= 2) {

                lastSpace = 0;
                reducedAmount = 0;
                rowCounter = 0;

                for (int i = 0; i < spacesAmountTwo; i++)
                {
                    if ((spaceIndexTwo[i] - reducedAmount) > maxLengthPerLine)
                    {
                        dexFormatTwo += "  \"" + dexDescriptionTwo.Substring(reducedAmount, lastSpace - reducedAmount) + "\\n\"\n";
                        reducedAmount += lastSpace - reducedAmount + 1;
                        rowCounter++;
                    }
                    if (i == spacesAmountTwo - 1)
                    {
                        dexFormatTwo += "  \"" + dexDescriptionTwo.Substring(reducedAmount) + "\"";
                    }
                    lastSpace = (spaceIndexTwo[i]);
                }

                if (rowCounter <= 2)
                {
                    pokemonName = comboBox1.Text;
                    pokemonName = (System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(pokemonName.ToLower())).Replace(" ", "");
                    if (pokemonName == "MrMime") { pokemonName = "Mrmime"; }

                    finalString = "static const u8 DexDescription_" + pokemonName + "_1[] = _(\n"
                        + dexFormatOne + ");" + "\nstatic const u8 DexDescription_" + pokemonName + "_2[] = _(\n"
                        + dexFormatTwo + ");\n";

                    string str = null;

                    StreamReader sr = new StreamReader(dictionary["pFile_pokedex_entries_en_h"].ToString());
                    str = sr.ReadToEnd();
                    sr.Close();
                    var index = 0;
                    var totalToChange = Regex.Matches(str, "static const u8 DexDescription_" + pokemonName + "_1").Cast<Match>().Count();
                    for (int i = 0; i < totalToChange; i++)
                    {
                        index = str.IndexOf("static const u8 DexDescription_" + pokemonName + "_1", index + 1);
                        var index2 = str.IndexOf(");", index) + 2;
                        index2 = str.IndexOf(");", index2) + 2;
                        var preStr = str.Substring(0, index);
                        var postStr = str.Substring(index2 + 1);
                        str = preStr + finalString + postStr;
                    }

                    data["pFile_pokedex_entries_en_h"] = str;

                    //richTextBox1.Text = str;

                    StreamWriter sw = new StreamWriter(dictionary["pFile_pokedex_entries_en_h"].ToString(), false);
                    sw.WriteLine(data["pFile_pokedex_entries_en_h"]);
                    sw.Close();

                }
                else
                {
                    MessageBox.Show("La página dos de la descripción excede la longitud máxima");
                }

            } else
            {
                MessageBox.Show("La página uno de la descripción excede la longitud máxima");
            }
        }

        private void setDexData()
        {
            var categoryName = categoriaPokemon.Text;
            var height = altura.Text;
            var weight = peso.Text;
            var pokemonScale = escalaPokemon.Text;
            var pokemonOffset = offsetPokemon.Text;
            var trainerScale = escalaEntrenador.Text;
            var trainerOffset = offsetEntrenador.Text;
            var pokemonSpecie = comboBox1.Text;
            var formatPokemonName = (System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(pokemonSpecie.ToLower())).Replace(" ", "");
            if (formatPokemonName == "MrMime")
            {
                formatPokemonName = "Mrmime";
            }

            var finalString = "    {  //" + formatPokemonName +
                "\n        .categoryName = _(\"" + categoryName + "\"),\n" +
                "        .height = " + height + ",\n" +
                "        .weight = " + weight + ",\n" +
                "        .descriptionPage1 = DexDescription_" + formatPokemonName + "_1,\n" +
                "        .descriptionPage2 = DexDescription_" + formatPokemonName + "_2,\n" +
                "        .pokemonScale = " + pokemonScale + ",\n" +
                "        .pokemonOffset = " + pokemonOffset + ",\n" +
                "        .trainerScale = " + trainerScale + ",\n" +
                "        .trainerOffset = " + trainerOffset + ",\n    },\n";

            //richTextBox1.Text = finalString;

            string str = null;

            StreamReader sr = new StreamReader(dictionary["pFile_pokedex_entries_en_h"].ToString());
            str = sr.ReadToEnd();
            sr.Close();

            var index = str.IndexOf("descriptionPage1 = DexDescription_" + formatPokemonName + "_1", 0);
            index = str.LastIndexOf("{", index) - 4;
            var index2 = str.IndexOf("},", index) + 2;
            var preStr = str.Substring(0, index);
            var postStr = str.Substring(index2 + 1);
            str = preStr + finalString + postStr;
            //richTextBox1.Text = str;
            data["pFile_pokedex_entries_en_h"] = str;

            //richTextBox1.Text = str;

            StreamWriter sw = new StreamWriter(dictionary["pFile_pokedex_entries_en_h"].ToString(), false);
            sw.WriteLine(data["pFile_pokedex_entries_en_h"]);
            sw.Close();

        }

        private void setSpriteData()
        {
            if (pokemonData["backCord"].ContainsKey(comboBox1.SelectedIndex.ToString()))
            {
                setBackCord();
            }
            if (pokemonData["frontCord"].ContainsKey(comboBox1.SelectedIndex.ToString()))
            {
                setFrontCord();
            }
            if (pokemonData["elevate"].ContainsKey(comboBox1.SelectedIndex.ToString()))
            {
                setElevation();
            }
            if (pokemonData["palUsed"].ContainsKey(comboBox1.SelectedIndex.ToString()))
            {
                setPalUsed();
            }
        }

        private void setBackCord()
        {
            string backCord = backY.Value.ToString();
            string firstByte = getSizeValue(pictureBox2.Image);
            if (backCord.Length == 1)
            {
                backCord = " " + backCord;
            }
            switch (firstByte.Length)
            {
                case 1:
                    firstByte = "  " + firstByte;
                    break;
                case 2:
                    firstByte = " " + firstByte;
                    break;
            }
            string finalString = ".byte " + firstByte + ", " + backCord + ", 0, 0";
            //MessageBox.Show(finalString);
            string str = null;

            StreamReader sr = new StreamReader(dictionary["pFile_back_pic_coords_inc"].ToString());
            str = sr.ReadToEnd();
            sr.Close();
            var index = 0;
            var pokemonIndex = comboBox1.SelectedIndex;

            for (int i = 0; i <= pokemonIndex; i++)
            {
                index = str.IndexOf(".byte", index + 1);
            }

            var getString = str.Substring(index, str.IndexOf("\n", index) - index);

            var preStr = str.Substring(0, index);
            var postStr = str.Substring(index + getString.Length);
            var resultString = preStr + finalString + postStr;

            //MessageBox.Show(getString);

            data["pFile_back_pic_coords_inc"] = resultString;

            //richTextBox1.Text = str;

            StreamWriter sw = new StreamWriter(dictionary["pFile_back_pic_coords_inc"].ToString(), false);
            sw.WriteLine(data["pFile_back_pic_coords_inc"]);
            sw.Close();
            //MessageBox.Show(finalString);
        }

        private void setFrontCord()
        {
            string frontCord = frontY.Value.ToString();
            string firstByte = getSizeValue(pictureBox3.Image);
            if (frontCord.Length == 1)
            {
                frontCord = " " + frontCord;
            }
            switch (firstByte.Length)
            {
                case 1:
                    firstByte = "  " + firstByte;
                    break;
                case 2:
                    firstByte = " " + firstByte;
                    break;
            }
            string finalString = ".byte " + firstByte + ", " + frontCord + ", 0, 0";

            

            string str = null;

            StreamReader sr = new StreamReader(dictionary["pFile_front_pic_coords_inc"].ToString());
            str = sr.ReadToEnd();
            sr.Close();
            var index = 0;
            var pokemonIndex = comboBox1.SelectedIndex;

            for (int i = 0; i <= pokemonIndex; i++)
            {
                index = str.IndexOf(".byte", index + 1);
            }

            var getString = str.Substring(index, str.IndexOf("\n", index) - index);

            var preStr = str.Substring(0, index);
            var postStr = str.Substring(index + getString.Length);

            var resultString = preStr + finalString + postStr;
            
            //MessageBox.Show(getString);

            data["pFile_front_pic_coords_inc"] = resultString;

            //richTextBox1.Text = str;

            StreamWriter sw = new StreamWriter(dictionary["pFile_front_pic_coords_inc"].ToString(), false);
            sw.WriteLine(data["pFile_front_pic_coords_inc"]);
            sw.Close();
            //MessageBox.Show(finalString);
        }

        private void setElevation()
        {
            string pokemonName = comboBox1.Text.ToUpper().Replace(" ", "_");

            string str = null;

            StreamReader sr = new StreamReader(dictionary["pFile_battle_1_c"].ToString());
            str = sr.ReadToEnd();
            sr.Close();

            var index = str.IndexOf("[SPECIES_" + pokemonName + "]", 0);

            var getString = str.Substring(index, str.IndexOf(",", index) - index);

            var levitation = Levitation.Value.ToString();

            if (levitation.Length == 1)
            {
                levitation = " " + levitation;
            }

            var preStr = str.Substring(0, index);
            var postStr = str.Substring(index + getString.Length);

            var resultString = preStr + "[SPECIES_" + pokemonName + "] = " + levitation + postStr;

            //MessageBox.Show(getString);
            
            data["pFile_battle_1_c"] = resultString;

            //richTextBox1.Text = str;

            StreamWriter sw = new StreamWriter(dictionary["pFile_battle_1_c"].ToString(), false);
            sw.WriteLine(data["pFile_battle_1_c"]);
            sw.Close();
        }

        private void setPalUsed()
        {
            string palUsed = iconPalette.Text.Replace(" ", "");

            string str = null;

            StreamReader sr = new StreamReader(dictionary["pFile_pokemon_icon_c"].ToString());
            str = sr.ReadToEnd();
            sr.Close();

            var index = str.IndexOf("const u8 gMonIconPaletteIndices[]", 0);
            var pokemonAmount = Regex.Matches(str, "gMonIcon").Cast<Match>().Count() - 1;
            index = str.IndexOf("{", index);
            for (int i = 0; i <= comboBox1.SelectedIndex; i++)
            {
                index = str.IndexOf("\n", index) + 1;
            }
            var pokemonFormat = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(comboBox1.Text.ToLower()).Replace(" ", "");
            var newString = "    " + palUsed + ", // " + pokemonFormat + "\n";
            var preStr = str.Substring(0, index);
            var postStr = str.Substring(str.IndexOf("\n", index) + 1);
            var finalString = preStr + newString + postStr;
            //MessageBox.Show(getString);

            data["pFile_pokemon_icon_c"] = finalString;

            //richTextBox1.Text = str;

            StreamWriter sw = new StreamWriter(dictionary["pFile_pokemon_icon_c"].ToString(), false);
            sw.WriteLine(data["pFile_pokemon_icon_c"]);
            sw.Close();
        }


        private void reloadAllSavedData()
        {
            pokemonData["psBase"][comboBox1.SelectedIndex.ToString()] = PS_Base.Text;
            pokemonData["ataqueBase"][comboBox1.SelectedIndex.ToString()] = ATQ_Base.Text;
            pokemonData["defensaBase"][comboBox1.SelectedIndex.ToString()] = DEF_Base.Text;
            pokemonData["velocidadBase"][comboBox1.SelectedIndex.ToString()] = VEL_Base.Text;
            pokemonData["ataqueEspecialBase"][comboBox1.SelectedIndex.ToString()] = ATESP_Base.Text;
            pokemonData["defensaEspecialBase"][comboBox1.SelectedIndex.ToString()] = DFESP_Base.Text;
            //Guardar tipo 1
            var formatoTipo1 = "TYPE_" + TIPO1.Text;
            formatoTipo1 = formatoTipo1.Replace(" ", "_");
            pokemonData["tipoUno"][comboBox1.SelectedIndex.ToString()] = formatoTipo1;
            //Recibir tipo 2
            var formatoTipo2 = "TYPE_" + TIPO2.Text;
            formatoTipo2 = formatoTipo2.Replace(" ", "_");
            pokemonData["tipoDos"][comboBox1.SelectedIndex.ToString()] = formatoTipo2;

            pokemonData["ratioDeCaptura"][comboBox1.SelectedIndex.ToString()] = ratioCaptura.Text;
            pokemonData["expBase"][comboBox1.SelectedIndex.ToString()] = expBase.Text;
            pokemonData["evsPS"][comboBox1.SelectedIndex.ToString()] = PS_Effort.Text;
            pokemonData["evsAtaque"][comboBox1.SelectedIndex.ToString()] = ATQ_Effort.Text;
            pokemonData["evsDefensa"][comboBox1.SelectedIndex.ToString()] = DEF_Effort.Text;
            pokemonData["evsVelocidad"][comboBox1.SelectedIndex.ToString()] = VEL_Effort.Text;
            pokemonData["evsAtaqueEspecial"][comboBox1.SelectedIndex.ToString()] = ATESP_Effort.Text;
            pokemonData["evsDefensaEspecial"][comboBox1.SelectedIndex.ToString()] = DFESP_Effort.Text;
            //Recibir objeto 1
            var formatoObjeto1 = "ITEM_" + OBJETO1.Text;
            formatoObjeto1 = formatoObjeto1.Replace(@" ", "_");
            pokemonData["objetoUno"][comboBox1.SelectedIndex.ToString()] = formatoObjeto1;
            //Recibir objeto 2
            var formatoObjeto2 = "ITEM_" + OBJETO2.Text;
            formatoObjeto2 = formatoObjeto2.Replace(@" ", "_");
            pokemonData["objetoDos"][comboBox1.SelectedIndex.ToString()] = formatoObjeto2;

            pokemonData["ratioGenero"][comboBox1.SelectedIndex.ToString()] = genero.Text;
            pokemonData["tieneGenero"][comboBox1.SelectedIndex.ToString()] = generoCheck.Checked.ToString();
            pokemonData["ciclosHuevo"][comboBox1.SelectedIndex.ToString()] = ciclosHuevo.Text;
            pokemonData["amistadBase"][comboBox1.SelectedIndex.ToString()] = amistadBase.Text;
            pokemonData["crecimiento"][comboBox1.SelectedIndex.ToString()] = "GROWTH_" + (crecimiento.Text).Replace(" ", "_");
            pokemonData["grupoHuevoUno"][comboBox1.SelectedIndex.ToString()] = "EGG_GROUP_" + (HUEVO1.Text).Replace(" ", "_");
            pokemonData["grupoHuevoDos"][comboBox1.SelectedIndex.ToString()] = "EGG_GROUP_" + (HUEVO2.Text).Replace(" ", "_");
            pokemonData["habilidadUno"][comboBox1.SelectedIndex.ToString()] = "ABILITY_" + (HABILIDAD1.Text).Replace(" ", "_");
            pokemonData["habilidadDos"][comboBox1.SelectedIndex.ToString()] = "ABILITY_" + (HABILIDAD2.Text).Replace(" ", "_");
            pokemonData["probabilidadHuidaSafari"][comboBox1.SelectedIndex.ToString()] = huidaSafari.Text;
            pokemonData["colorCuerpo"][comboBox1.SelectedIndex.ToString()] = "BODY_COLOR_" + (COLOR_CUERPO.Text).Replace(" ", "_");
            pokemonData["pokemonName"][comboBox1.SelectedIndex.ToString()] = POKEMON_NAME.Text.Replace(" ", "_");
            //EVOLUTION
            var evosAmount = this.dataGridView2.Rows.Count - 1;
            string method;
            string argument;
            string evolution;

            for (int i = 0; i < evosAmount; i++)
            {
                method = "EVO_" + (this.dataGridView2.Rows[i].Cells[0].Value).ToString().Replace(" ", "_");
                argument = (this.dataGridView2.Rows[i].Cells[1].Value).ToString();
                evolution = "SPECIES_" + (this.dataGridView2.Rows[i].Cells[2].Value).ToString().Replace(" ", "_");
                //MessageBox.Show(method + "\n" + argument + "\n" + evolution);
                evolutionData[comboBox1.SelectedIndex.ToString() + "_" + i.ToString()] = Tuple.Create(evosAmount.ToString(), method, argument, evolution);
            }
            //MOVEMENTS
            var moveAmount = this.dataGridView1.Rows.Count - 1;
            string level;
            string movement;

            for (int i = 0; i < moveAmount; i++)
            {
                level = this.dataGridView1.Rows[i].Cells[1].Value.ToString();
                movement = "MOVE_" + this.dataGridView1.Rows[i].Cells[0].Value.ToString().Replace(" ", "_");
                moveData[comboBox1.SelectedIndex.ToString() + "_" + i.ToString()] = Tuple.Create(moveAmount.ToString(), level, movement);
            }
            //MT/MO
            var mtmoAmount = this.dataGridView3.Rows.Count - 1;
            string mtmo;

            for (int i = 0; i < mtmoAmount; i++)
            {
                mtmo = this.dataGridView3.Rows[i].Cells[0].Value.ToString().Replace(" ", "_");
                mtmoData[comboBox1.SelectedIndex.ToString() + "_" + i.ToString()] = Tuple.Create(mtmoAmount.ToString(), mtmo);
            }
            //POKEDEX INFORMATION
            pokemonData["pokedexPageOne"][comboBox1.SelectedIndex.ToString()] = descripcionUno.Text;
            pokemonData["pokedexPageTwo"][comboBox1.SelectedIndex.ToString()] = descripcionDos.Text;
            pokemonData["categoriaPokemon"][comboBox1.SelectedIndex.ToString()] = categoriaPokemon.Text;
            pokemonData["altura"][comboBox1.SelectedIndex.ToString()] = altura.Text;
            pokemonData["peso"][comboBox1.SelectedIndex.ToString()] = peso.Text;
            pokemonData["escalaPokemon"][comboBox1.SelectedIndex.ToString()] = escalaPokemon.Text;
            pokemonData["offsetPokemon"][comboBox1.SelectedIndex.ToString()] = offsetPokemon.Text;
            pokemonData["escalaEntrenador"][comboBox1.SelectedIndex.ToString()] = escalaEntrenador.Text;
            pokemonData["offsetEntrenador"][comboBox1.SelectedIndex.ToString()] = offsetEntrenador.Text;
            pokemonData["palUsed"][comboBox1.SelectedIndex.ToString()] = iconPalette.Text;

            //SPRITES
            if (pokemonData["frontCord"].ContainsKey(comboBox1.SelectedIndex.ToString()))
            {
                pokemonData["frontCord"][comboBox1.SelectedIndex.ToString()] = frontY.Value.ToString();
            }
            if (pokemonData["backCord"].ContainsKey(comboBox1.SelectedIndex.ToString()))
            {
                pokemonData["backCord"][comboBox1.SelectedIndex.ToString()] = backY.Value.ToString();
            }
            if (pokemonData["elevate"].ContainsKey(comboBox1.SelectedIndex.ToString()))
            {
                pokemonData["elevate"][comboBox1.SelectedIndex.ToString()] = Levitation.Value.ToString();
            }
        }

        private void setSpritePosition()
        {
            pictureBox2.Location = new Point(40, 48 + Int32.Parse((backY.Value).ToString()));
            pictureBox3.Location = new Point(144, 8 + Int32.Parse((frontY.Value).ToString()) - Int32.Parse((Levitation.Value).ToString()));
            if (Levitation.Value > 0)
            {
                fondo.Image = Properties.Resources.bgSombra;
            } else
            {
                fondo.Image = Properties.Resources.bg1;
            }
        }


        // * * * * * * * * * * * //
        // Generar nuevo Pokémon //
        // * * * * * * * * * * * //

        private void generarPokemon()
        {
            DialogResult dr = MessageBox.Show("¿Inserción masiva?",
                      "Modo de inserción", MessageBoxButtons.YesNo);

            switch (dr)
            {
                case DialogResult.Yes:
                    insercionMasiva();
                    comboBox1.SelectedIndex = comboBox1.Items.Count - 1;
                    break;
                case DialogResult.No:
                    insercionIndividual();
                    comboBox1.SelectedIndex = comboBox1.Items.Count - 1;
                    break;
            }
        }

        private string insercionIndividual()
        {
            NombrePokemon f = new NombrePokemon();
            if (f.ShowDialog() == DialogResult.OK)
            {
                minusPokemonName = (f.PkmnName).ToLower();
                mayusPokemonName = (f.PkmnName).ToUpper().Replace(" ", "_");
                firstMayusPokemonName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(f.PkmnName.ToLower()).Replace(" ", "");
                var checkPokemon = mayusPokemonName;
                if (checkPokemon == "NIDORAN_M")
                {
                    checkPokemon = "NIDORAN♂";
                } else if (checkPokemon == "NIDORAN_F")
                {
                    checkPokemon = "NIDORAN♀";
                }
                if (pokemonData["pokemonName"].Any(tr => tr.Value.Equals(checkPokemon, StringComparison.CurrentCultureIgnoreCase)) == false)
                {
                    species_h_set();
                    global_h_set();
                    base_stats_h_set();
                    cry_ids_h_set();
                    level_up_learnset_pointers_set();
                    level_up_learnset_set();
                    tmhm_learnset_set();
                    pokedex_orders_set();
                    pokedex_entries_set();
                    species_names_set();
                    pokedex_c_set();
                    pokedex_h_set();
                    pokemon_1_set();
                    pokemon_icon_set();
                    graphics_set();
                    back_pic_table_set();
                    front_pic_table_set();
                    graphics_inc_set();
                    palette_table_set();
                    shiny_palette_table_set();
                    back_pick_coords_set();
                    front_pick_coords_set();
                    battle_1_set();
                    direct_sound_data_set();
                    data_reset();
                    return "true";
                } else
                {
                    MessageBox.Show("Este pokémon ya existe, prueba con otro nombre.");
                    return "false";
                }
                
            } else
            {
                return "break";
            }
            
        }

        private void insercionMasiva()
        {
            Datos f = new Datos();
            if (f.ShowDialog() == DialogResult.OK)
            {
                var newPokes = f.PkmnAmount;
                for (int i = 0; i < newPokes; i++)
                {
                    var result = insercionIndividual();
                    if (result == "false") { i--; } else if (result == "break"){ break; }
                }
            }
        }

        private void species_h_set()
        {
            string str = null;
            StreamReader sr = new StreamReader(dictionary["pFile_species_h"].ToString());
            str = sr.ReadToEnd();
            sr.Close();
            var index = 0;
            var lastIndex = index;
            string preStr = null;
            string postStr = null;
            int indexEgg = str.IndexOf("#define SPECIES_EGG", index);

            //Change SPECIES

            int eggValue = Int32.Parse(str.Substring((indexEgg + 20), str.IndexOf("\n", indexEgg) - (indexEgg + 20)));
            eggValue--;
            index = str.IndexOf(eggValue.ToString(), index) + eggValue.ToString().Length;
            eggValue++;
            preStr = str.Substring(0, index);
            postStr = str.Substring(str.IndexOf("#define NUM_SPECIES SPECIES_EGG", 0) - 1);
            var afterValue = resetAfterValues(eggValue);
            //richTextBox1.Text = preStr + "\n#define SPECIES_" + mayusPokemonName + " " + (eggValue) + afterValue + postStr;
            str = preStr + "\n#define SPECIES_" + mayusPokemonName + " " + (eggValue) + afterValue + postStr;

            //Change NATIONAL_DEX

            index = 0;
            var unownBIndex = str.IndexOf("#define NATIONAL_DEX_OLD_UNOWN_B", 0) + 33;
            var unownValue = str.Substring(unownBIndex, (str.IndexOf("\n", unownBIndex)) - unownBIndex);

            var secondString = "#define NATIONAL_DEX_" + mayusPokemonName + " " + unownValue;
            var nationalAmount = Regex.Matches(str, "#define NATIONAL_DEX_").Cast<Match>().Count() - 25;
            int plusValue = Int32.Parse(unownValue);
            string highSecondString = null;

            index = str.IndexOf("#define NATIONAL_DEX_CELEBI", 0);

            for (int i = 0; i < 25; i++)
            {
                var unownWorkString = str.Substring(str.IndexOf("#define NATIONAL_DEX_", index + 1), str.IndexOf(" ", str.IndexOf("NATIONAL_DEX", str.IndexOf("#define", index + 1))) - str.IndexOf("#define NATIONAL_DEX_", index + 1));
                index = str.IndexOf("#define NATIONAL_DEX_", index + 1);
                plusValue++;
                highSecondString += unownWorkString + " " + plusValue + "\n";
            }
            index = 0;
            preStr = str.Substring(0, str.IndexOf("#define NATIONAL_DEX_OLD_UNOWN_B", index));
            postStr = str.Substring(str.IndexOf("\n", str.IndexOf("#define NATIONAL_DEX_OLD_UNOWN_Z", index)) + 1);
            str = preStr + highSecondString + postStr;

            preStr = str.Substring(0, str.IndexOf("\n", str.LastIndexOf("#define NATIONAL_DEX_")));
            postStr = str.Substring(str.IndexOf("\n", str.LastIndexOf("#define NATIONAL_DEX_")));
            str = preStr + "\n" + secondString + postStr;
            //richTextBox1.Text = str;

            //Change HOENN_DEX

            var unownBIndex2 = str.IndexOf("#define HOENN_DEX_OLD_UNOWN_B", 0) + 30;
            var unownValue2 = str.Substring(unownBIndex2, (str.IndexOf("\n", unownBIndex2)) - unownBIndex2);

            var thirdString = "#define HOENN_DEX_" + mayusPokemonName + " " + unownValue2;
            var hoennAmount = Regex.Matches(str, "#define HOENN_DEX_").Cast<Match>().Count() - 25;
            plusValue = Int32.Parse(unownValue2);
            string highThirdString = null;

            index = str.IndexOf("#define HOENN_DEX_OLD_UNOWN_B", 0) - 1;

            for (int i = 0; i < 25; i++)
            {
                var unownWorkString = str.Substring(str.IndexOf("#define HOENN_DEX_", index + 1), str.IndexOf(" ", str.IndexOf("HOENN_DEX_", str.IndexOf("#define", index + 1))) - str.IndexOf("#define HOENN_DEX_", index + 1));
                index = str.IndexOf("#define HOENN_DEX_", index + 1);
                plusValue++;
                highThirdString += unownWorkString + " " + plusValue + "\n";
            }
            index = 0;
            preStr = str.Substring(0, str.IndexOf("#define HOENN_DEX_OLD_UNOWN_B", index) - 2);
            postStr = str.Substring(str.IndexOf("\n", str.IndexOf("#define HOENN_DEX_OLD_UNOWN_Z", index)) + 1);
            str = preStr + "\n" + thirdString + "\n\n" + highThirdString + postStr;
            //richTextBox1.Text = str;

            data["pFile_species_h"] = str;

            //richTextBox1.Text = str;

            StreamWriter sw = new StreamWriter(dictionary["pFile_species_h"].ToString(), false);
            sw.WriteLine(data["pFile_species_h"]);
            sw.Close();

        }

        private void global_h_set()
        {
            string str = null;
            StreamReader sr = new StreamReader(dictionary["pFile_global_h"].ToString());
            str = sr.ReadToEnd();
            sr.Close();

            var actualNumber = Int32.Parse(str.Substring((str.IndexOf("#define POKEMON_SLOTS_NUMBER", 0) + 29), str.IndexOf("\n", str.IndexOf("#define POKEMON_SLOTS_NUMBER", 0) + 1) - (str.IndexOf("#define POKEMON_SLOTS_NUMBER", 0) + 29)));
            var preStr = str.Substring(0, str.IndexOf("#define POKEMON_SLOTS_NUMBER", 0));
            var postStr = str.Substring(str.IndexOf("\n", str.IndexOf("#define POKEMON_SLOTS_NUMBER", 0)));
            actualNumber++;
            var finalString = preStr + "#define POKEMON_SLOTS_NUMBER " + actualNumber + postStr;
            //richTextBox1.Text = finalString;

            data["pFile_global_h"] = finalString;

            //richTextBox1.Text = str;

            StreamWriter sw = new StreamWriter(dictionary["pFile_global_h"].ToString(), false);
            sw.WriteLine(data["pFile_global_h"]);
            sw.Close();
        }

        private void base_stats_h_set()
        {
            string str = null;
            StreamReader sr = new StreamReader(dictionary["pFile_base_stats_h"].ToString());
            str = sr.ReadToEnd();
            sr.Close();

            var index = str.IndexOf("};", 0) - 1;
            var baseStatsString = ",\n\n    [SPECIES_" + mayusPokemonName + "] =" +
                "\n    {" +
                "\n        .baseHP        = 0," +
                "\n        .baseAttack    = 0," +
                "\n        .baseDefense   = 0," +
                "\n        .baseSpeed     = 0," +
                "\n        .baseSpAttack  = 0," +
                "\n        .baseSpDefense = 0," +
                "\n        .type1 = TYPE_NORMAL," +
                "\n        .type2 = TYPE_NORMAL," +
                "\n        .catchRate = 0," +
                "\n        .expYield = 0," +
                "\n        .evYield_HP        = 0," +
                "\n        .evYield_Attack    = 0," +
                "\n        .evYield_Defense   = 0," +
                "\n        .evYield_Speed     = 0," +
                "\n        .evYield_SpAttack  = 0," +
                "\n        .evYield_SpDefense = 0," +
                "\n        .item1 = ITEM_NONE," +
                "\n        .item2 = ITEM_NONE," +
                "\n        .genderRatio = PERCENT_FEMALE(50)," +
                "\n        .eggCycles = 0," +
                "\n        .friendship = 0," +
                "\n        .growthRate = GROWTH_MEDIUM_FAST," +
                "\n        .eggGroup1 = EGG_GROUP_FIELD," +
                "\n        .eggGroup2 = EGG_GROUP_FIELD," +
                "\n        .ability1 = ABILITY_NONE," +
                "\n        .ability2 = ABILITY_NONE," +
                "\n        .safariZoneFleeRate = 0," +
                "\n        .bodyColor = BODY_COLOR_GRAY," +
                "\n        .noFlip = FALSE," +
                "\n    }";

            var preStr = str.Substring(0, index);
            var postStr = str.Substring(index);
            var finalString = preStr + baseStatsString + postStr;
            //richTextBox1.Text = finalString;

            data["pFile_base_stats_h"] = finalString;

            //richTextBox1.Text = str;

            StreamWriter sw = new StreamWriter(dictionary["pFile_base_stats_h"].ToString(), false);
            sw.WriteLine(data["pFile_base_stats_h"]);
            sw.Close();
        }

        private void cry_ids_h_set()
        {
            string str = null;
            StreamReader sr = new StreamReader(dictionary["pFile_cry_ids_h"].ToString());
            str = sr.ReadToEnd();
            sr.Close();
            var valueIndex = str.IndexOf("\n", str.LastIndexOf(",")) + 5;
            //MessageBox.Show(str.Substring(valueIndex, str.IndexOf(" ", valueIndex) - valueIndex));
            var pokemonValue = Int32.Parse(str.Substring(valueIndex, str.IndexOf(" ", valueIndex) - valueIndex));
            pokemonValue++;
            var index = str.IndexOf(" ", str.IndexOf("\n", str.LastIndexOf(",")) + 5);
            var finalString = str.Substring(0, index) + "," + str.Substring(index + 1);
            str = finalString;

            index = str.IndexOf("};") - 1;
            var newString = "\n    " + pokemonValue + "  // " + mayusPokemonName;
            var preStr = str.Substring(0, index);
            var postStr = str.Substring(index);
            finalString = preStr + newString + postStr;
            //richTextBox1.Text = finalString;

            data["pFile_cry_ids_h"] = finalString;

            //richTextBox1.Text = str;

            StreamWriter sw = new StreamWriter(dictionary["pFile_cry_ids_h"].ToString(), false);
            sw.WriteLine(data["pFile_cry_ids_h"]);
            sw.Close();
        }

        private void level_up_learnset_pointers_set()
        {
            string str = null;
            StreamReader sr = new StreamReader(dictionary["pFile_level_up_learnset_pointers_h"].ToString());
            str = sr.ReadToEnd();
            sr.Close();

            //INDEX DE LA ÚLTIMA POSICIÓN
            string toSearch = "};";
            int index = str.LastIndexOf(toSearch) - 1;
            //
            string newString = ",\n    g" + firstMayusPokemonName + "LevelUpLearnset";
            var preStr = str.Substring(0, index);
            var postStr = str.Substring(index);
            var finalString = preStr + newString + postStr;
            //richTextBox1.Text = finalString;

            data["pFile_level_up_learnset_pointers_h"] = finalString;

            //richTextBox1.Text = str;

            StreamWriter sw = new StreamWriter(dictionary["pFile_level_up_learnset_pointers_h"].ToString(), false);
            sw.WriteLine(data["pFile_level_up_learnset_pointers_h"]);
            sw.Close();

        }

        private void level_up_learnset_set()
        {
            string str = null;
            StreamReader sr = new StreamReader(dictionary["pFile_level_up_learnsets_h"].ToString());
            str = sr.ReadToEnd();
            sr.Close();

            //INDEX DE LA ÚLTIMA POSICIÓN
            string toSearch = "};";
            int index = str.LastIndexOf(toSearch) + 2;
            //
            string newString = "\n\nconst u16 g" + firstMayusPokemonName + "LevelUpLearnset[] = {\n" +
                "    LEVEL_UP_MOVE( 1, MOVE_TACKLE),\n" +
                "    LEVEL_UP_END\n};";
            var preStr = str.Substring(0, index);
            var postStr = str.Substring(index);
            var finalString = preStr + newString + postStr;
            //richTextBox1.Text = finalString;

            data["pFile_level_up_learnsets_h"] = finalString;

            //richTextBox1.Text = str;

            StreamWriter sw = new StreamWriter(dictionary["pFile_level_up_learnsets_h"].ToString(), false);
            sw.WriteLine(data["pFile_level_up_learnsets_h"]);
            sw.Close();
        }

        private void tmhm_learnset_set()
        {
            string str = null;
            StreamReader sr = new StreamReader(dictionary["pFile_tmhm_learnsets_h"].ToString());
            str = sr.ReadToEnd();
            sr.Close();

            //INDEX DE LA ÚLTIMA POSICIÓN
            string toSearch = "};";
            int index = str.LastIndexOf(toSearch) - 2;
            //"    [SPECIES_CATERPIE]    = TMHM_LEARNSET(0),"
            string newString = "\n\n    [SPECIES_" + mayusPokemonName + "]";
            for (int i = 0; i < (12 - mayusPokemonName.Length); i++)
            {
                newString += " ";
            }
            newString += "= TMHM_LEARNSET(0),";
            var preStr = str.Substring(0, index);
            var postStr = str.Substring(index);
            var finalString = preStr + newString + postStr;
            //richTextBox1.Text = finalString;
            
            data["pFile_tmhm_learnsets_h"] = finalString;

            //richTextBox1.Text = str;

            StreamWriter sw = new StreamWriter(dictionary["pFile_tmhm_learnsets_h"].ToString(), false);
            sw.WriteLine(data["pFile_tmhm_learnsets_h"]);
            sw.Close();
        }

        private void pokedex_orders_set()
        {
            string str = null;
            StreamReader sr = new StreamReader(dictionary["pFile_pokedex_orders_h"].ToString());
            str = sr.ReadToEnd();
            sr.Close();
            String[] pokeNames = new string[(pokemonData["pokemonName"].Count + 1)];
            for (int i = 0; i < pokemonData["pokemonName"].Count; i++)
            {
                pokeNames[i] = pokemonData["pokemonName"][(i + 1).ToString()];
            }
            pokeNames[pokemonData["pokemonName"].Count] = mayusPokemonName;
            var sortedNames = pokeNames.OrderBy(n => n);
            string[] pokeNameOrdered = sortedNames.ToArray();
            var newPokeIndex = Array.FindIndex(pokeNameOrdered, row => row.ToString() == mayusPokemonName);
            if (pokeNameOrdered[newPokeIndex + 1].ToString() == "MR. MIME"){pokeNameOrdered[newPokeIndex + 1] = "MR MIME";}
            var index = str.IndexOf("NATIONAL_DEX_" + pokeNameOrdered[newPokeIndex + 1].ToString().Replace(" ", "_"), 0);
            //MessageBox.Show(("NATIONAL_DEX_" + pokeNameOrdered[newPokeIndex + 1]).Replace(" ", "n"));
            var newString = "NATIONAL_DEX_" + mayusPokemonName;
            var preStr = str.Substring(0, index);
            var postStr = str.Substring(index);
            var finalString = preStr + newString + ",\n    " + postStr;
            str = finalString;
            index = str.IndexOf("static const u16 gPokedexOrder_Weight[] =", 0);
            index = str.IndexOf("{", index) + 1;
            preStr = str.Substring(0, index);
            postStr = str.Substring(index);
            finalString = preStr + "\n    " + newString + "," + postStr;
            str = finalString;
            index = str.IndexOf("static const u16 gPokedexOrder_Height[] =", 0);
            index = str.IndexOf("{", index) + 1;
            preStr = str.Substring(0, index);
            postStr = str.Substring(index);
            finalString = preStr + "\n    " + newString + "," + postStr;
            //richTextBox1.Text = finalString;

            data["pFile_pokedex_orders_h"] = finalString;

            //richTextBox1.Text = str;

            StreamWriter sw = new StreamWriter(dictionary["pFile_pokedex_orders_h"].ToString(), false);
            sw.WriteLine(data["pFile_pokedex_orders_h"]);
            sw.Close();
        }

        private void pokedex_entries_set()
        {
            string str = null;
            StreamReader sr = new StreamReader(dictionary["pFile_pokedex_entries_en_h"].ToString());
            str = sr.ReadToEnd();
            sr.Close();

            //INDEX DE LA ÚLTIMA POSICIÓN
            string toSearch = "const struct PokedexEntry gPokedexEntries[] =";
            int index = str.LastIndexOf(toSearch) - 2;

            string newString = "\n\nstatic const u8 DexDescription_" + firstMayusPokemonName + "_1[] = _(" +
                "\n  \"\");" +
                "\nstatic const u8 DexDescription_" + firstMayusPokemonName + "_2[] = _(" +
                "\n  \"\");";
            string preStr = str.Substring(0, index);
            string postStr = str.Substring(index);
            string finalString = preStr + newString + postStr;
            str = finalString;
            //richTextBox1.Text = finalString;

            index = str.LastIndexOf("};") - 1;
            newString = "    {  //" + firstMayusPokemonName +
                "\n        .categoryName = _(\"NULL\")," +
                "\n        .height = 0," +
                "\n        .weight = 0," +
                "\n        .descriptionPage1 = DexDescription_" + firstMayusPokemonName + "_1," +
                "\n        .descriptionPage2 = DexDescription_" + firstMayusPokemonName + "_2," +
                "\n        .pokemonScale = 0," +
                "\n        .pokemonOffset = 0," +
                "\n        .trainerScale = 0," +
                "\n        .trainerOffset = 0," +
                "\n    },";
            preStr = str.Substring(0, index);
            postStr = str.Substring(index);
            finalString = preStr + "\n" + newString + postStr;
            //richTextBox1.Text = finalString;

            data["pFile_pokedex_entries_en_h"] = finalString;

            //richTextBox1.Text = str;

            StreamWriter sw = new StreamWriter(dictionary["pFile_pokedex_entries_en_h"].ToString(), false);
            sw.WriteLine(data["pFile_pokedex_entries_en_h"]);
            sw.Close();
        }

        private void species_names_set()
        {
            string str = null;
            StreamReader sr = new StreamReader(dictionary["pFile_species_names_en_h"].ToString());
            str = sr.ReadToEnd();
            sr.Close();

            //INDEX DE LA ÚLTIMA POSICIÓN
            string toSearch = "};";
            int index = str.LastIndexOf(toSearch) - 1;
            string newString = "\n    [SPECIES_" + mayusPokemonName + "] = _(\"" + mayusPokemonName + "\"),";
            string preStr = str.Substring(0, index);
            string postStr = str.Substring(index);
            string finalString = preStr + newString + postStr;
            //richTextBox1.Text = finalString;

            data["pFile_species_names_en_h"] = finalString;

            //richTextBox1.Text = str;

            StreamWriter sw = new StreamWriter(dictionary["pFile_species_names_en_h"].ToString(), false);
            sw.WriteLine(data["pFile_species_names_en_h"]);
            sw.Close();
        }

        private void pokedex_c_set()
        {
            string str = null;
            StreamReader sr = new StreamReader(dictionary["pFile_pokedex_c"].ToString());
            str = sr.ReadToEnd();
            sr.Close();
            int index;
            string newString;
            string preStr;
            string postStr;
            string finalString;

            //INDEX DE LA ÚLTIMA POSICIÓN
            string toSearch = "#define NATIONAL_DEX_COUNT";
            if (str.IndexOf(toSearch, 0) != -1)
            {
                index = str.IndexOf(toSearch, 0) + 27;
                int newInt = Int32.Parse(str.Substring(index, (str.IndexOf("\n", index) - index)));
                newInt++;
                newString = newInt.ToString();
                preStr = str.Substring(0, index);
                index = str.IndexOf("\n", index);
                postStr = str.Substring(index);
                finalString = preStr + newString + postStr;
                str = finalString;
            }
            index = str.LastIndexOf("gMonFootprint_Bulbasaur,") - 5;
            newString = "\n    gMonFootprint_" + firstMayusPokemonName + ",";
            preStr = str.Substring(0, index);
            postStr = str.Substring(index);
            finalString = preStr + newString + postStr;
            //richTextBox1.Text = finalString;

            data["pFile_pokedex_c"] = finalString;

            //richTextBox1.Text = str;

            StreamWriter sw = new StreamWriter(dictionary["pFile_pokedex_c"].ToString(), false);
            sw.WriteLine(data["pFile_pokedex_c"]);
            sw.Close();
        }

        private void pokedex_h_set()
        {
            string str = null;
            StreamReader sr = new StreamReader(dictionary["pFile_pokedex_h"].ToString());
            str = sr.ReadToEnd();
            sr.Close();

            //INDEX DE LA ÚLTIMA POSICIÓN
            string toSearch = "#define NATIONAL_DEX_COUNT";
            int index = str.IndexOf(toSearch, 0) + 27;
            int newInt = Int32.Parse(str.Substring(index, (str.IndexOf("\n", index) - index)));
            newInt++;
            var newString = newInt.ToString();
            var preStr = str.Substring(0, index);
            index = str.IndexOf("\n", index);
            var postStr = str.Substring(index);
            var finalString = preStr + newString + postStr;
            //richTextBox1.Text = finalString;
            
            data["pFile_pokedex_h"] = finalString;

            //richTextBox1.Text = str;

            StreamWriter sw = new StreamWriter(dictionary["pFile_pokedex_h"].ToString(), false);
            sw.WriteLine(data["pFile_pokedex_h"]);
            sw.Close();
        }

        private void pokemon_1_set()
        {
            string str = null;
            StreamReader sr = new StreamReader(dictionary["pFile_pokemon_1_c"].ToString());
            str = sr.ReadToEnd();
            sr.Close();

            //INDEX DE LA ÚLTIMA POSICIÓN
            string toSearch = "gSpeciesToHoennPokedexNum";
            int index = str.IndexOf(toSearch);
            index = str.IndexOf("HOENN_DEX_OLD_UNOWN_B,", index) - 2;
            string newString = "\n\tHOENN_DEX_" + mayusPokemonName + ",";
            for (int i = 0; i < (17 - mayusPokemonName.Length); i++)
            {
                newString += " ";
            }
            newString += "// SPECIES_" + mayusPokemonName;
            string preStr = str.Substring(0, index);
            string postStr = str.Substring(index);
            string finalString = preStr + newString + postStr;
            str = finalString;

            index = str.IndexOf("const u16 gSpeciesToNationalPokedexNum[]", 0);
            var index2 = str.IndexOf("};", index) + 2;
            var workString = str.Substring(index, index2 - index);
            var workIndex = workString.LastIndexOf("NATIONAL_DEX_");
            var workHalfString = workString.Substring(workIndex, workString.IndexOf(" ", workIndex) - workIndex) + ",";
            var preWorkStr = workString.Substring(0, workIndex);
            var postWorkStr = workString.Substring(workString.IndexOf(" ", workIndex));
            var finalWorkStr = preWorkStr + workHalfString + postWorkStr;
            preStr = str.Substring(0, index);
            postStr = str.Substring(index2);
            finalString = preStr + finalWorkStr + postStr;
            str = finalString;

            index = str.IndexOf("};", index) - 1;
            newString = "\n\tNATIONAL_DEX_" + mayusPokemonName;
            for (int i = 0; i < (19 - mayusPokemonName.Length); i++)
            {
                newString += " ";
            }
            newString += "// SPECIES_" + mayusPokemonName;
            preStr = str.Substring(0, index);
            postStr = str.Substring(index);
            finalString = preStr + newString + postStr;
            str = finalString;

            index = str.IndexOf("gHoennToNationalOrder", 0);
            index = str.IndexOf("NATIONAL_DEX_OLD_UNOWN_B,", index) - 2;
            newString = "\n\tNATIONAL_DEX_" + mayusPokemonName + ",";
            for (int i = 0; i < (14 - mayusPokemonName.Length); i++)
            {
                newString += " ";
            }
            newString += "// SPECIES_" + mayusPokemonName;
            preStr = str.Substring(0, index);
            postStr = str.Substring(index);
            finalString = preStr + newString + postStr;
            //richTextBox1.Text = finalString;

            data["pFile_pokemon_1_c"] = finalString;

            //richTextBox1.Text = str;

            StreamWriter sw = new StreamWriter(dictionary["pFile_pokemon_1_c"].ToString(), false);
            sw.WriteLine(data["pFile_pokemon_1_c"]);
            sw.Close();
        }

        private void pokemon_icon_set()
        {
            string str = null;
            StreamReader sr = new StreamReader(dictionary["pFile_pokemon_icon_c"].ToString());
            str = sr.ReadToEnd();
            sr.Close();

            //INDEX DE LA ÚLTIMA POSICIÓN
            string toSearch = "const u8 * const gMonIconTable[]";
            int index = str.LastIndexOf(toSearch) - 1;
            index = str.IndexOf("    gMonIcon_Egg,", index) - 1;
            var newString = "\n    gMonIcon_" + firstMayusPokemonName + ",";
            var preStr = str.Substring(0, index);
            var postStr = str.Substring(index);
            var finalString = preStr + newString + postStr;
            str = finalString;

            index = str.IndexOf("gMonIconPaletteIndices", index);
            index = str.IndexOf("    1, // Egg", index) - 1;
            newString = "\n    0, // " + firstMayusPokemonName;
            preStr = str.Substring(0, index);
            postStr = str.Substring(index);
            finalString = preStr + newString + postStr;
            //richTextBox1.Text = finalString;

            data["pFile_pokemon_icon_c"] = finalString;

            //richTextBox1.Text = str;

            StreamWriter sw = new StreamWriter(dictionary["pFile_pokemon_icon_c"].ToString(), false);
            sw.WriteLine(data["pFile_pokemon_icon_c"]);
            sw.Close();
        }

        private void graphics_set()
        {
            string str = null;
            StreamReader sr = new StreamReader(dictionary["pFile_graphics_h"].ToString());
            str = sr.ReadToEnd();
            sr.Close();

            //INDEX DE LA ÚLTIMA POSICIÓN
            string toSearch = "extern const u8 gMonPic_Egg[];";
            int index = str.LastIndexOf(toSearch) - 1;
            string newString = "\nextern const u8 gMonFrontPic_" + firstMayusPokemonName + "[];" +
                "\nextern const u8 gMonPalette_" + firstMayusPokemonName + "[];" +
                "\nextern const u8 gMonBackPic_" + firstMayusPokemonName + "[];" +
                "\nextern const u8 gMonShinyPalette_" + firstMayusPokemonName + "[];" +
                "\nextern const u8 gMonIcon_" + firstMayusPokemonName + "[];" +
                "\nextern const u8 gMonFootprint_" + firstMayusPokemonName + "[];";
            var preStr = str.Substring(0, index);
            var postStr = str.Substring(index);
            var finalString = preStr + newString + postStr;
            //richTextBox1.Text = finalString;

            data["pFile_graphics_h"] = finalString;

            //richTextBox1.Text = str;

            StreamWriter sw = new StreamWriter(dictionary["pFile_graphics_h"].ToString(), false);
            sw.WriteLine(data["pFile_graphics_h"]);
            sw.Close();
        }

        private void back_pic_table_set()
        {
            string str = null;
            StreamReader sr = new StreamReader(dictionary["pFile_back_pic_table_inc"].ToString());
            str = sr.ReadToEnd();
            sr.Close();

            //INDEX DE LA ÚLTIMA POSICIÓN
            string toSearch = "\tobj_tiles gMonPic_Egg";
            int index = str.LastIndexOf(toSearch) - 1;
            var newString = "\n\tobj_tiles gMonBackPic_" + firstMayusPokemonName + ", 0x800, SPECIES_" + mayusPokemonName;
            var preStr = str.Substring(0, index);
            var postStr = str.Substring(index);
            var finalString = preStr + newString + postStr;
            //richTextBox1.Text = finalString;

            data["pFile_back_pic_table_inc"] = finalString;

            //richTextBox1.Text = str;

            StreamWriter sw = new StreamWriter(dictionary["pFile_back_pic_table_inc"].ToString(), false);
            sw.WriteLine(data["pFile_back_pic_table_inc"]);
            sw.Close();
        }

        private void front_pic_table_set()
        {
            string str = null;
            StreamReader sr = new StreamReader(dictionary["pFile_front_pic_table_inc"].ToString());
            str = sr.ReadToEnd();
            sr.Close();

            //INDEX DE LA ÚLTIMA POSICIÓN
            string toSearch = "\tobj_tiles gMonPic_Egg";
            int index = str.LastIndexOf(toSearch) - 1;
            var newString = "\n\tobj_tiles gMonFrontPic_" + firstMayusPokemonName + ", 0x800, SPECIES_" + mayusPokemonName;
            var preStr = str.Substring(0, index);
            var postStr = str.Substring(index);
            var finalString = preStr + newString + postStr;
            //richTextBox1.Text = finalString;

            data["pFile_front_pic_table_inc"] = finalString;

            //richTextBox1.Text = str;

            StreamWriter sw = new StreamWriter(dictionary["pFile_front_pic_table_inc"].ToString(), false);
            sw.WriteLine(data["pFile_front_pic_table_inc"]);
            sw.Close();
        }

        private void graphics_inc_set()
        {
            //pFile_graphics_h
            string str = null;
            StreamReader sr = new StreamReader(dictionary["pFile_graphics_inc"].ToString());
            str = sr.ReadToEnd();
            sr.Close();

            //INDEX DE LA ÚLTIMA POSICIÓN
            string toSearch = "\t.align 2\ngMonPic_Egg";
            int index = str.LastIndexOf(toSearch) - 1;
            string newString = "\n\t.align 2\ngMonFrontPic_" + firstMayusPokemonName + "::" +
                "\n\t.incbin \"graphics/pokemon/" + minusPokemonName + "/front.4bpp.lz\"\n" +
                "\n\t.align 2" +
                "\ngMonPalette_" + firstMayusPokemonName + "::" +
                "\n\t.incbin \"graphics/pokemon/" + minusPokemonName + "/normal.gbapal.lz\"\n" +
                "\n\t.align 2" +
                "\ngMonBackPic_" + firstMayusPokemonName + "::" +
                "\n\t.incbin \"graphics/pokemon/" + minusPokemonName + "/back.4bpp.lz\"\n" +
                "\n\t.align 2" +
                "\ngMonShinyPalette_" + firstMayusPokemonName + "::" +
                "\n\t.incbin \"graphics/pokemon/" + minusPokemonName + "/shiny.gbapal.lz\"\n" +
                "\n\t.align 2" +
                "\ngMonIcon_" + firstMayusPokemonName + "::" +
                "\n\t.incbin \"graphics/pokemon/" + minusPokemonName + "/icon.4bpp\"\n" +
                "\n\t.align 2" +
                "\ngMonFootprint_" + firstMayusPokemonName + "::" +
                "\n\t.incbin \"graphics/pokemon/" + minusPokemonName + "/footprint.1bpp\"\n";
            string preStr = str.Substring(0, index);
            string postStr = str.Substring(index);
            string finalString = preStr + newString + postStr;
            //richTextBox1.Text = finalString;

            data["pFile_graphics_inc"] = finalString;

            //richTextBox1.Text = str;

            StreamWriter sw = new StreamWriter(dictionary["pFile_graphics_inc"].ToString(), false);
            sw.WriteLine(data["pFile_graphics_inc"]);
            sw.Close();
        }

        private void palette_table_set()
        {
            string str = null;
            StreamReader sr = new StreamReader(dictionary["pFile_palette_table_inc"].ToString());
            str = sr.ReadToEnd();
            sr.Close();

            //INDEX DE LA ÚLTIMA POSICIÓN
            string toSearch = "\tobj_pal gMonPalette_Egg";
            int index = str.LastIndexOf(toSearch) - 1;
            var newString = "\n\tobj_pal gMonPalette_" + firstMayusPokemonName + ", SPECIES_" + mayusPokemonName;
            var preStr = str.Substring(0, index);
            var postStr = str.Substring(index);
            var finalString = preStr + newString + postStr;
            //richTextBox1.Text = finalString;

            data["pFile_palette_table_inc"] = finalString;

            //richTextBox1.Text = str;

            StreamWriter sw = new StreamWriter(dictionary["pFile_palette_table_inc"].ToString(), false);
            sw.WriteLine(data["pFile_palette_table_inc"]);
            sw.Close();
        }

        private void shiny_palette_table_set()
        {
            string str = null;
            StreamReader sr = new StreamReader(dictionary["pFile_shiny_palette_table_inc"].ToString());
            str = sr.ReadToEnd();
            sr.Close();

            //INDEX DE LA ÚLTIMA POSICIÓN
            string toSearch = "\tobj_pal gMonPalette_Egg";
            int index = str.LastIndexOf(toSearch) - 1;
            var newString = "\n\tobj_pal gMonShinyPalette_" + firstMayusPokemonName + ", PAL_ID_SHINY + SPECIES_" + mayusPokemonName;
            var preStr = str.Substring(0, index);
            var postStr = str.Substring(index);
            var finalString = preStr + newString + postStr;
            //richTextBox1.Text = finalString;

            data["pFile_shiny_palette_table_inc"] = finalString;

            //richTextBox1.Text = str;

            StreamWriter sw = new StreamWriter(dictionary["pFile_shiny_palette_table_inc"].ToString(), false);
            sw.WriteLine(data["pFile_shiny_palette_table_inc"]);
            sw.Close();
        }

        private void back_pick_coords_set()
        {
            string str = null;
            StreamReader sr = new StreamReader(dictionary["pFile_back_pic_coords_inc"].ToString());
            str = sr.ReadToEnd();
            sr.Close();

            var backBytes = Regex.Matches(str, ".byte").Cast<Match>().Count() - 28;
            var index = 0;
            for (int i = 0; i < backBytes; i++)
            {
                index = str.IndexOf(".byte", index + 1);
            }
            index = str.IndexOf("\n", index);
            var newString = "\n\t.byte   0,  0, 0, 0";
            var preStr = str.Substring(0, index);
            var postStr = str.Substring(index);
            var finalString = preStr + newString + postStr;
            //richTextBox1.Text = finalString;

            data["pFile_back_pic_coords_inc"] = finalString;

            //richTextBox1.Text = str;

            StreamWriter sw = new StreamWriter(dictionary["pFile_back_pic_coords_inc"].ToString(), false);
            sw.WriteLine(data["pFile_back_pic_coords_inc"]);
            sw.Close();

        }

        private void front_pick_coords_set()
        {
            string str = null;
            StreamReader sr = new StreamReader(dictionary["pFile_front_pic_coords_inc"].ToString());
            str = sr.ReadToEnd();
            sr.Close();

            var frontBytes = Regex.Matches(str, ".byte").Cast<Match>().Count() - 28;
            var index = 0;
            for (int i = 0; i < frontBytes; i++)
            {
                index = str.IndexOf(".byte", index + 1);
            }
            index = str.IndexOf("\n", index);
            var newString = "\n\t.byte   0,  0, 0, 0";
            var preStr = str.Substring(0, index);
            var postStr = str.Substring(index);
            var finalString = preStr + newString + postStr;
            //richTextBox1.Text = finalString;
            
            data["pFile_front_pic_coords_inc"] = finalString;

            //richTextBox1.Text = str;

            StreamWriter sw = new StreamWriter(dictionary["pFile_front_pic_coords_inc"].ToString(), false);
            sw.WriteLine(data["pFile_front_pic_coords_inc"]);
            sw.Close();
        }

        private void battle_1_set()
        {
            string str = null;
            StreamReader sr = new StreamReader(dictionary["pFile_battle_1_c"].ToString());
            str = sr.ReadToEnd();
            sr.Close();

            //INDEX DE LA ÚLTIMA POSICIÓN
            string toSearch = "const u8 gEnemyMonElevation[]";
            int index = str.LastIndexOf(toSearch) - 1;
            index = str.IndexOf("};", index) - 1;
            var newString = "\n    [SPECIES_" + mayusPokemonName + "] = 0,";
            var preStr = str.Substring(0, index);
            var postStr = str.Substring(index);
            var finalString = preStr + newString + postStr;
            //richTextBox1.Text = finalString;
            data["pFile_battle_1_c"] = finalString;

            //richTextBox1.Text = str;

            StreamWriter sw = new StreamWriter(dictionary["pFile_battle_1_c"].ToString(), false);
            sw.WriteLine(data["pFile_battle_1_c"]);
            sw.Close();
        }

        private void direct_sound_data_set()
        {
            string str = null;
            StreamReader sr = new StreamReader(dictionary["pFile_direct_sound_data_inc"].ToString());
            str = sr.ReadToEnd();
            sr.Close();

            //INDEX DE LA ÚLTIMA POSICIÓN
            string toSearch = "\t.align 2\nDirectSoundWaveData_Phoneme_Vowel7";
            int index = str.LastIndexOf(toSearch) - 1;
            string newString = "\n\t.align 2\nCry_" + firstMayusPokemonName + "::\n\t.incbin \"sound/direct_sound_samples/cries/cry_" + minusPokemonName + ".bin\"\n";
            string preStr = str.Substring(0, index);
            string postStr = str.Substring(index);
            string finalString = preStr + newString + postStr;
            //richTextBox1.Text = finalString;

            data["pFile_direct_sound_data_inc"] = finalString;

            //richTextBox1.Text = str;

            StreamWriter sw = new StreamWriter(dictionary["pFile_direct_sound_data_inc"].ToString(), false);
            sw.WriteLine(data["pFile_direct_sound_data_inc"]);
            sw.Close();
        }

        private void data_reset()
        {
            var newIndex = comboBox1.Items.Count;

            pokemonData["psBase"][newIndex.ToString()] = "0";
            pokemonData["ataqueBase"][newIndex.ToString()] = "0";
            pokemonData["defensaBase"][newIndex.ToString()] = "0";
            pokemonData["velocidadBase"][newIndex.ToString()] = "0";
            pokemonData["ataqueEspecialBase"][newIndex.ToString()] = "0";
            pokemonData["defensaEspecialBase"][newIndex.ToString()] = "0";
            pokemonData["tipoUno"][newIndex.ToString()] = "TYPE_NORMAL";
            pokemonData["tipoDos"][newIndex.ToString()] = "TYPE_NORMAL";
            pokemonData["ratioDeCaptura"][newIndex.ToString()] = "0";
            pokemonData["expBase"][newIndex.ToString()] = "0";
            pokemonData["evsPS"][newIndex.ToString()] = "0";
            pokemonData["evsAtaque"][newIndex.ToString()] = "0";
            pokemonData["evsDefensa"][newIndex.ToString()] = "0";
            pokemonData["evsVelocidad"][newIndex.ToString()] = "0";
            pokemonData["evsAtaqueEspecial"][newIndex.ToString()] = "0";
            pokemonData["evsDefensaEspecial"][newIndex.ToString()] = "0";
            pokemonData["objetoUno"][newIndex.ToString()] = "ITEM_NONE";
            pokemonData["objetoDos"][newIndex.ToString()] = "ITEM_NONE";
            pokemonData["ratioGenero"][newIndex.ToString()] = "50";
            pokemonData["tieneGenero"][newIndex.ToString()] = "true";
            pokemonData["ciclosHuevo"][newIndex.ToString()] = "0";
            pokemonData["amistadBase"][newIndex.ToString()] = "0";
            pokemonData["crecimiento"][newIndex.ToString()] = "GROWTH_MEDIUM_FAST";
            pokemonData["grupoHuevoUno"][newIndex.ToString()] = "EGG_GROUP_FIELD";
            pokemonData["grupoHuevoDos"][newIndex.ToString()] = "EGG_GROUP_FIELD";
            pokemonData["habilidadUno"][newIndex.ToString()] = "ABILITY_NONE";
            pokemonData["habilidadDos"][newIndex.ToString()] = "ABILITY_NONE";
            pokemonData["probabilidadHuidaSafari"][newIndex.ToString()] = "0";
            pokemonData["colorCuerpo"][newIndex.ToString()] = "BODY_COLOR_GRAY";
            pokemonData["pokemonName"][newIndex.ToString()] = mayusPokemonName;
            moveData[newIndex.ToString() + "_0"] = Tuple.Create("1", "1", "MOVE_TACKLE");
            pokemonData["pokedexPageOne"][newIndex.ToString()] = "";
            pokemonData["pokedexPageTwo"][newIndex.ToString()] = "";
            pokemonData["categoriaPokemon"][newIndex.ToString()] = "NULL";
            pokemonData["altura"][newIndex.ToString()] = "0";
            pokemonData["peso"][newIndex.ToString()] = "0";
            pokemonData["escalaPokemon"][newIndex.ToString()] = "0";
            pokemonData["offsetPokemon"][newIndex.ToString()] = "0";
            pokemonData["escalaEntrenador"][newIndex.ToString()] = "0";
            pokemonData["offsetEntrenador"][newIndex.ToString()] = "0";
            pokemonData["frontCord"][newIndex.ToString()] = "0";
            pokemonData["backCord"][newIndex.ToString()] = "0";
            pokemonData["elevate"][newIndex.ToString()] = "0";
            pokemonData["palUsed"][newIndex.ToString()] = "0";
            comboBox1.Items.Add(mayusPokemonName);
            comboBox2.Items.Add(mayusPokemonName);
            comboBox3.Items.Add(mayusPokemonName);
        }

        private string resetAfterValues(int actualValue)
        {
            int counter = actualValue + 1;
            string newValues = "\n#define SPECIES_EGG " + counter;
            counter++;
            newValues += "\n\n#define SPECIES_UNOWN_B " + counter;
            counter++;
            newValues += "\n#define SPECIES_UNOWN_C " + counter;
            counter++;
            newValues += "\n#define SPECIES_UNOWN_D " + counter;
            counter++;
            newValues += "\n#define SPECIES_UNOWN_E " + counter;
            counter++;
            newValues += "\n#define SPECIES_UNOWN_F " + counter;
            counter++;
            newValues += "\n#define SPECIES_UNOWN_G " + counter;
            counter++;
            newValues += "\n#define SPECIES_UNOWN_H " + counter;
            counter++;
            newValues += "\n#define SPECIES_UNOWN_I " + counter;
            counter++;
            newValues += "\n#define SPECIES_UNOWN_J " + counter;
            counter++;
            newValues += "\n#define SPECIES_UNOWN_K " + counter;
            counter++;
            newValues += "\n#define SPECIES_UNOWN_L " + counter;
            counter++;
            newValues += "\n#define SPECIES_UNOWN_M " + counter;
            counter++;
            newValues += "\n#define SPECIES_UNOWN_N " + counter;
            counter++;
            newValues += "\n#define SPECIES_UNOWN_O " + counter;
            counter++;
            newValues += "\n#define SPECIES_UNOWN_P " + counter;
            counter++;
            newValues += "\n#define SPECIES_UNOWN_Q " + counter;
            counter++;
            newValues += "\n#define SPECIES_UNOWN_R " + counter;
            counter++;
            newValues += "\n#define SPECIES_UNOWN_S " + counter;
            counter++;
            newValues += "\n#define SPECIES_UNOWN_T " + counter;
            counter++;
            newValues += "\n#define SPECIES_UNOWN_U " + counter;
            counter++;
            newValues += "\n#define SPECIES_UNOWN_V " + counter;
            counter++;
            newValues += "\n#define SPECIES_UNOWN_W " + counter;
            counter++;
            newValues += "\n#define SPECIES_UNOWN_X " + counter;
            counter++;
            newValues += "\n#define SPECIES_UNOWN_Y " + counter;
            counter++;
            newValues += "\n#define SPECIES_UNOWN_Z " + counter;
            counter++;
            newValues += "\n#define SPECIES_UNOWN_EMARK " + counter;
            counter++;
            newValues += "\n#define SPECIES_UNOWN_QMARK " + counter + "\n";
            return newValues;
        }
    }
}

//MUCHIO ESPANIOL
                 
                 