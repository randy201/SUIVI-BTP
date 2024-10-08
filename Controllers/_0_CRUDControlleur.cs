using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using temp_back.Models;

namespace temp_back.Controllers
{
    [ApiController]
    [Route("0_CRUD")]
    public class _0_CRUDControlleur : ControllerBase
    {
        public Type type { get; set; } = typeof(Type_finition);
        //Nom de table = Route affichage
        public string filePath = @"D:\00ITU\DOSSIER S6\00-prep\prep-eval\";

        [HttpGet("creationList")]
        public async Task<IActionResult> creationList(string nomtable)
        {
            Console.WriteLine($"-----------generation crud List----------- path= {filePath}");
            filePath = filePath + "List_" + nomtable + ".jsx";
            string nomclasse = type.ToString().Split('.')[2].ToLower();
            string nomClasse = type.ToString().Split('.')[2];
            // Vérifiez si le fichier n'existe pas déjà
            if (System.IO.File.Exists(filePath))
            {
                return BadRequest("Le fichier existe deja");
            }

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                //ecriture import
                writer.WriteLine(
                    "import { useContext, useEffect, useState } from \"react\";" +
                    "import Pagination from \"../Pagination\";" +
                    "import { Link } from \"react-router-dom\";" +
                    "import axios from \"axios\";" +
                    "import httpErrorHandler from \"../HttpError\";" +
                    "import Apicontext from \"../../context/api-context\";" +
                    "import { format } from \"date-fns\"; " +
                    "import { fr } from \"date-fns/locale\";"
                    );

                writer.WriteLine(
                    "export default function List_" + nomtable + "() {" +
                    "const [" + nomclasse + ", set" + nomClasse + "] = useState([]);" +
                    "const [message, setMessage] = useState(\"\");" +
                    "const URL = useContext(Apicontext);"
                    );

                //ecriture fonction
                writer.WriteLine("//Les fonctions");
                writer.WriteLine(
                    "async function getData() {" +
                        "try {" +
                            "const url = `${URL}/" + nomtable + "`;" +
                            "let response = await axios.get(url);" +
                            "response = response.data;" +
                            "set" + nomClasse + "(response);" +
                        "} catch (error) {" + "setMessage(httpErrorHandler(error));}" +
                    "}");

                writer.WriteLine("useEffect(() => { getData(); }, []);");


                //Filtre des donné -----------------------------------------------------
                writer.WriteLine("//donné filtration des données");
                foreach (PropertyInfo property in type.GetProperties())
                {
                    if (property.PropertyType == typeof(IFormFile) || Nullable.GetUnderlyingType(property.PropertyType) == typeof(IFormFile))
                    {
                        continue;
                    }

                    // Écriture du nom de l'attribut dans le fichier
                    writer.WriteLine("const [filtre" + property.Name + ", setFiltre" + property.Name + "] = useState(false);");
                }
                writer.WriteLine("const sort" + nomClasse + " = [..." + nomclasse + "];");
                foreach (PropertyInfo property in type.GetProperties())
                {
                    if (property.PropertyType == typeof(string)
                        || property.PropertyType == typeof(DateTime)
                        || property.PropertyType == typeof(DateOnly)
                        || property.PropertyType == typeof(TimeOnly)
                        || property.PropertyType == typeof(DateTimeOffset)
                        || Nullable.GetUnderlyingType(property.PropertyType) == typeof(string)
                        || Nullable.GetUnderlyingType(property.PropertyType) == typeof(DateTime)
                        || Nullable.GetUnderlyingType(property.PropertyType) == typeof(DateOnly)
                        || Nullable.GetUnderlyingType(property.PropertyType) == typeof(TimeOnly)
                        || Nullable.GetUnderlyingType(property.PropertyType) == typeof(DateTimeOffset)
                        )
                    {
                        // Écriture du nom de l'attribut dans le fichier
                        writer.WriteLine("if (filtre" + property.Name + ") {sort"
                                         + nomClasse + ".sort((a,b)=> a." + property.Name.ToLower() + ".localeCompare(b." + property.Name.ToLower() + ")); }");
                    }
                    else if (property.PropertyType == typeof(IFormFile)
                        || Nullable.GetUnderlyingType(property.PropertyType) == typeof(IFormFile))
                    {
                        writer.WriteLine("");
                    }
                    else
                    {
                        // Écriture du nom de l'attribut dans le fichier
                        writer.WriteLine("if (filtre" + property.Name + ") {sort"
                                         + nomClasse + ".sort((a,b)=> a." + property.Name.ToLower() + " - b." + property.Name.ToLower() + "); }");
                    }
                }
                // recherche --------------------------------------------------
                writer.WriteLine("//recherche");
                writer.WriteLine("const [searchTerm, setSearchTerm] = useState(\"\");");
                writer.WriteLine(" let filteredData = sort" + nomClasse + ".filter(");
                writer.WriteLine("(" + nomclasse + ") => ");
                foreach (PropertyInfo property in type.GetProperties())
                {
                    if (property.PropertyType == typeof(int)
                        || Nullable.GetUnderlyingType(property.PropertyType) == typeof(int)
                        || property.PropertyType == typeof(double)
                        || Nullable.GetUnderlyingType(property.PropertyType) == typeof(double)
                        || property.PropertyType == typeof(decimal)
                       )
                    {
                        writer.WriteLine(nomclasse + "." + property.Name.ToLower() + ".toString().includes(searchTerm.toString().toLowerCase())");

                    }
                    else if (property.PropertyType == typeof(IFormFile) ||
                             Nullable.GetUnderlyingType(property.PropertyType) == typeof(IFormFile))
                    {
                        writer.WriteLine("");
                    }
                    else
                    {
                        writer.WriteLine(nomclasse + "." + property.Name.ToLower() + ".toLowerCase().includes(searchTerm.toLowerCase())");
                    }
                    if (property != type.GetProperties()[type.GetProperties().Length - 1])
                    {
                        writer.Write("||");
                    }
                }
                writer.WriteLine(");");
                //Pagination --------------------------------
                writer.WriteLine(
                    "\n //Pagination \n" +
                    "const [currentPage, setCurrentPage] = useState(1);" +
                    "const [itemsPerPage] = useState(3);" +
                    "\n // Fonction pour paginer les elements \n" +
                    "const paginate = (pageNumber) => setCurrentPage(pageNumber);" +
                    "const indexOfLastItem = currentPage * itemsPerPage;" +
                    "const indexOfFirstItem = indexOfLastItem - itemsPerPage;" +
                    "const currentItems = filteredData.slice(indexOfFirstItem, indexOfLastItem);"
                    );

                writer.Write(
                    "return ( \n" +
                    "<div className=\"body-element\" style={{ justifyContent: \"space-evenly\" }}>" +
                    "<div style={{ color: \"red\" }}>{message && JSON.stringify(message)}</div>" +
                        "<div style={{ display: \"flex\", justifyContent: \"end\" }}>" +
                            "<input type=\"text\" placeholder=\"Rechercher\" value={searchTerm} onChange={(e) => setSearchTerm(e.target.value)}/>" +
                        "</div> \n" +
                        "<table style={{ width: \"100%\" }}>" +
                            "<thead>" +
                                "<tr>");
                foreach (PropertyInfo property in type.GetProperties())
                {
                    // Écriture du nom de l'attribut dans le fichier
                    writer.Write("<th onClick={()=>{ ");
                    foreach (PropertyInfo property2 in type.GetProperties())
                    {
                        writer.Write("setFiltre" + property2.Name + "(");
                        if (property2 == property)
                        {
                            writer.Write("true");
                        }
                        else
                        {
                            writer.Write("false");
                        }
                        writer.Write(");");
                    }
                    writer.Write("}}>" + property.Name + "</th>");
                }
                writer.WriteLine("<th></th>" + "</tr>" + "</thead>");
                writer.WriteLine("<tbody>");
                writer.WriteLine("{currentItems.map((" + nomclasse + ") => (");
                writer.WriteLine("<tr key={" + nomclasse + "." + type.GetProperties()[0].Name.ToLower() + "}>");
                foreach (PropertyInfo property in type.GetProperties())
                {
                    // Écriture du nom de l'attribut dans le fichier
                    if (property.PropertyType == typeof(DateOnly)
                        || Nullable.GetUnderlyingType(property.PropertyType) == typeof(DateOnly))
                    {
                        writer.WriteLine("<td>{format(" + nomclasse + "." + property.Name.ToLower() + ",\"EEEE, dd MMMM yyyy\", {locale: fr})}</td>");
                    }
                    else if (property.PropertyType == typeof(IFormFile)
                             || Nullable.GetUnderlyingType(property.PropertyType) == typeof(IFormFile))
                    {
                        writer.WriteLine("<img src=\"\" alt=\"image_" + property.Name.ToLower() + "\" />");

                    }
                    else if (property.PropertyType == typeof(DateTime)
                             || Nullable.GetUnderlyingType(property.PropertyType) == typeof(DateTime))
                    {
                        writer.WriteLine("<td>{format(" + nomclasse + "." + property.Name.ToLower() + ",\"EEEE, dd MMMM yyyy 'à' HH:mm\",{locale: fr})}</td>");

                    }
                    else { writer.WriteLine("<td>{" + nomclasse + "." + property.Name.ToLower() + "}</td>"); }
                }
                writer.WriteLine(
                    "<td>" +
                        "<div className=\"actions\">" +
                            "<Link to={`detail/${" + nomclasse + "." + type.GetProperties()[0].Name.ToLower() + "}`} className=\"modifier\">" +
                                "<span className=\"mdi mdi-draw \"></span>" +
                            "</Link>" +
                            "<Link to={`supprimer/${" + nomclasse + "." + type.GetProperties()[0].Name.ToLower() + "}`} className=\"supprimer\">" +
                                "<span className=\"mdi mdi-delete-empty \"></span>" +
                            "</Link>" +
                        "</div>" +
                    "</td>"
                );

                writer.WriteLine(
                                "</tr>" +
                            "))}" +
                        "</tbody>" +
                        "</table>" +
                        "<div className=\"pagination\">" +
                            "<Pagination " + "itemsPerPage={itemsPerPage}" + "totalItems={" + nomclasse + ".length}" + "paginate={paginate}" + "/>" +
                        "</div>" +
                    "</div>);");
                writer.WriteLine("}");
            }

            return Ok("Fichier créer avec succès");
        }


        [HttpGet("creationListPagination")]
        public async Task<IActionResult> creationListPagination(string nomtable)
        {
            Console.WriteLine($"-----------generation crud List----------- path= {filePath}");
            filePath = filePath + "List_" + nomtable + ".jsx";
            string nomclasse = type.ToString().Split('.')[2].ToLower();
            string nomClasse = type.ToString().Split('.')[2];
            // Vérifiez si le fichier n'existe pas déjà
            if (System.IO.File.Exists(filePath))
            {
                return BadRequest("Le fichier existe deja");
            }

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                //ecriture import
                writer.WriteLine(
                    "import { useContext, useEffect, useState } from \"react\";" +
                    "import { Link } from \"react-router-dom\";" +
                    "import axios from \"axios\";" +
                    "import httpErrorHandler from \"../HttpError\";" +
                    "import Apicontext from \"../../context/api-context\";" +
                    "import { format } from \"date-fns\"; " +
                    "import { fr } from \"date-fns/locale\";"
                    );

                writer.WriteLine(
                    "export default function List_" + nomtable + "() {" +
                    "const [" + nomclasse + ", set" + nomClasse + "] = useState([]);" +
                    "const [message, setMessage] = useState(\"\");" +
                    "const URL = useContext(Apicontext);" +
                    "const [suivant, setSuivant] = useState(false);" +
                    "const [page, setPage] = useState(1);"
                    );

                //ecriture fonction
                writer.WriteLine("//Les fonctions");
                writer.WriteLine(
                    "async function getData() {" +
                        "try {" +
                            "const url = `${URL}/" + nomtable + "/pagination?page=${page}`;" +
                            "let response = await axios.get(url);" +
                            "response = response.data;" +
                            "set" + nomClasse + "(response.data);" +
                            "setSuivant(response.suivant);" +
                        "} catch (error) {" + "setMessage(httpErrorHandler(error));}" +
                    "}");

                writer.WriteLine("useEffect(() => { getData(); }, [page]);");


                //Filtre des donné -----------------------------------------------------
                writer.WriteLine("//donné filtration des données");
                foreach (PropertyInfo property in type.GetProperties())
                {
                    if (property.PropertyType == typeof(IFormFile) || Nullable.GetUnderlyingType(property.PropertyType) == typeof(IFormFile))
                    {
                        continue;
                    }

                    // Écriture du nom de l'attribut dans le fichier
                    writer.WriteLine("const [filtre" + property.Name + ", setFiltre" + property.Name + "] = useState(false);");
                }
                writer.WriteLine("const sort" + nomClasse + " = [..." + nomclasse + "];");
                foreach (PropertyInfo property in type.GetProperties())
                {
                    if (property.PropertyType == typeof(string)
                        || property.PropertyType == typeof(DateTime)
                        || property.PropertyType == typeof(DateOnly)
                        || property.PropertyType == typeof(TimeOnly)
                        || property.PropertyType == typeof(DateTimeOffset)
                        || Nullable.GetUnderlyingType(property.PropertyType) == typeof(string)
                        || Nullable.GetUnderlyingType(property.PropertyType) == typeof(DateTime)
                        || Nullable.GetUnderlyingType(property.PropertyType) == typeof(DateOnly)
                        || Nullable.GetUnderlyingType(property.PropertyType) == typeof(TimeOnly)
                        || Nullable.GetUnderlyingType(property.PropertyType) == typeof(DateTimeOffset)
                        )
                    {
                        // Écriture du nom de l'attribut dans le fichier
                        writer.WriteLine("if (filtre" + property.Name + ") {sort"
                                         + nomClasse + ".sort((a,b)=> a." + property.Name.ToLower() + ".localeCompare(b." + property.Name.ToLower() + ")); }");
                    }
                    else if (property.PropertyType == typeof(IFormFile)
                        || Nullable.GetUnderlyingType(property.PropertyType) == typeof(IFormFile))
                    {
                        writer.WriteLine("");
                    }
                    else
                    {
                        // Écriture du nom de l'attribut dans le fichier
                        writer.WriteLine("if (filtre" + property.Name + ") {sort"
                                         + nomClasse + ".sort((a,b)=> a." + property.Name.ToLower() + " - b." + property.Name.ToLower() + "); }");
                    }
                }
                // recherche --------------------------------------------------
                writer.WriteLine("//recherche");
                writer.WriteLine("const [searchTerm, setSearchTerm] = useState(\"\");");
                writer.WriteLine(" let filteredData = sort" + nomClasse + ".filter(");
                writer.WriteLine("(" + nomclasse + ") => ");
                foreach (PropertyInfo property in type.GetProperties())
                {
                    if (property.PropertyType == typeof(int)
                        || Nullable.GetUnderlyingType(property.PropertyType) == typeof(int)
                        || property.PropertyType == typeof(double)
                        || Nullable.GetUnderlyingType(property.PropertyType) == typeof(double)
                        || property.PropertyType == typeof(decimal)
                       )
                    {
                        writer.WriteLine(nomclasse + "." + property.Name.ToLower() + ".toString().includes(searchTerm.toString().toLowerCase())");

                    }
                    else if (property.PropertyType == typeof(IFormFile) ||
                             Nullable.GetUnderlyingType(property.PropertyType) == typeof(IFormFile))
                    {
                        writer.WriteLine("");
                    }
                    else
                    {
                        writer.WriteLine(nomclasse + "." + property.Name.ToLower() + ".toLowerCase().includes(searchTerm.toLowerCase())");
                    }
                    if (property != type.GetProperties()[type.GetProperties().Length - 1])
                    {
                        writer.Write("||");
                    }
                }
                writer.WriteLine(");");

                writer.Write(
                    "return ( \n" +
                    "<div className=\"body-element\" style={{ justifyContent: \"space-evenly\" }}>" +
                        "<div style={{ color: \"red\" }}>{message && JSON.stringify(message)}</div>" +
                        "<div style={{ display: \"flex\", justifyContent: \"end\" }}>" +
                            "<input type=\"text\" placeholder=\"Rechercher\" value={searchTerm} onChange={(e) => setSearchTerm(e.target.value)}/>" +
                        "</div> \n" +
                        "<table style={{ width: \"100%\" }}>" +
                            "<thead>" +
                                "<tr>");
                foreach (PropertyInfo property in type.GetProperties())
                {
                    // Écriture du nom de l'attribut dans le fichier
                    writer.Write("<th onClick={()=>{ ");
                    foreach (PropertyInfo property2 in type.GetProperties())
                    {
                        writer.Write("setFiltre" + property2.Name + "(");
                        if (property2 == property)
                        {
                            writer.Write("true");
                        }
                        else
                        {
                            writer.Write("false");
                        }
                        writer.Write(");");
                    }
                    writer.Write("}}>" + property.Name + "</th>");
                }
                writer.WriteLine("<th></th>" + "</tr>" + "</thead>");
                writer.WriteLine("<tbody>");
                writer.WriteLine("{filteredData.map((" + nomclasse + ") => (");
                writer.WriteLine("<tr key={" + nomclasse + "." + type.GetProperties()[0].Name.ToLower() + "}>");
                foreach (PropertyInfo property in type.GetProperties())
                {
                    // Écriture du nom de l'attribut dans le fichier
                    if (property.PropertyType == typeof(DateOnly)
                        || Nullable.GetUnderlyingType(property.PropertyType) == typeof(DateOnly))
                    {
                        writer.WriteLine("<td>{format(" + nomclasse + "." + property.Name.ToLower() + ",\"EEEE, dd MMMM yyyy\", {locale: fr})}</td>");
                    }
                    else if (property.PropertyType == typeof(IFormFile)
                             || Nullable.GetUnderlyingType(property.PropertyType) == typeof(IFormFile))
                    {
                        writer.WriteLine("<img src=\"\" alt=\"image_" + property.Name.ToLower() + "\" />");

                    }
                    else if (property.PropertyType == typeof(DateTime)
                             || Nullable.GetUnderlyingType(property.PropertyType) == typeof(DateTime))
                    {
                        writer.WriteLine("<td>{format(" + nomclasse + "." + property.Name.ToLower() + ",\"EEEE, dd MMMM yyyy 'à' HH:mm\",{locale: fr})}</td>");

                    }
                    else { writer.WriteLine("<td>{" + nomclasse + "." + property.Name.ToLower() + "}</td>"); }
                }
                writer.WriteLine(
                    "<td>" +
                        "<div className=\"actions\">" +
                            "<Link to={`detail/${" + nomclasse + "." + type.GetProperties()[0].Name.ToLower() + "}`} className=\"modifier\">" +
                                "<span className=\"mdi mdi-draw \"></span>" +
                            "</Link>" +
                            "<Link to={`supprimer/${" + nomclasse + "." + type.GetProperties()[0].Name.ToLower() + "}`} className=\"supprimer\">" +
                                "<span className=\"mdi mdi-delete-empty \"></span>" +
                            "</Link>" +
                        "</div>" +
                    "</td>"
                );

                writer.WriteLine(
                                "</tr>" +
                            "))}" +
                        "</tbody>" +
                        "</table>" +
                        "<div className=\"pagination\">" +
                                "{page > 1 && (<button className={`element-pagination`} onClick={() => setPage(page - 1)} >Précédant</button> )}{suivant ? ( <button className={`element-pagination`} onClick={() => setPage(page + 1)} >Suivant</button> ) : null}" +
                        "</div>" +
                    "</div>);");
                writer.WriteLine("}");
            }

            return Ok("Fichier créer avec succès");
        }

        [HttpGet("creationList_Import")]
        public async Task<IActionResult> creationListImport(string nomtable)
        {
            Console.WriteLine($"-----------generation crud List----------- path= {filePath}");
            filePath = filePath + "List_" + nomtable + ".jsx";
            string nomclasse = type.ToString().Split('.')[2].ToLower();
            string nomClasse = type.ToString().Split('.')[2];
            // Vérifiez si le fichier n'existe pas déjà
            if (System.IO.File.Exists(filePath))
            {
                return BadRequest("Le fichier existe deja");
            }

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                //ecriture import
                writer.WriteLine(
                    "import Apicontext from \"../../context/api-context\";" +
                    "import axios from \"axios\";" +
                    "import { useContext, useState } from \"react\";" +
                    "import { useParams } from \"react-router-dom\";" +
                    "import httpErrorHandler from \"../HttpError\";"
                );

                //ecriture attribut
                writer.WriteLine(
                    "export default function List_" + nomtable + "_CSV({data}) {" +
                    "const URL = useContext(Apicontext);" +
                    "const [message, setMessage] = useState(\"\");" +
                    "const [" + nomclasse + "_csv] = useState(data);");

                writer.WriteLine(
                    "const sendData = async () => {" +
                        "try {const url = `${URL}/" + nomClasse + "/add_csv`;" +
                            "let response = await axios.post(url, data);" +
                            "if (response.status == 200) {" + "console.table(data);" + "alert(\"succes\");" +
                        "}" +
                        "} catch (error) {alert(httpErrorHandler(error));}" +
                    "};");

                writer.WriteLine(
                        "return (" +
                            "<div className=\"body-element\" style={{ justifyContent: \"space-evenly\" }}>"
                    );
                writer.Write(
                        "{data ? (<><table style={{ width: \"100%\" }}><thead><tr>"
                    );
                foreach (PropertyInfo property in type.GetProperties())
                {
                    writer.WriteLine("<th>" + property.Name.ToLower() + "</th>");
                }
                writer.WriteLine("</tr></thead><tbody>");
                writer.WriteLine(
                    "{data.map((datacsv) => (<tr key={datacsv." + type.GetProperties()[0].Name + "}>");
                foreach (PropertyInfo property in type.GetProperties())
                {
                    writer.WriteLine("<td>{datacsv." + property.Name.ToLower() + "}</td>");
                }

                writer.WriteLine(
                    "</tr>))}</tbody></table><button onClick={sendData}>Importer les données</button></>) : (\"Les donnés son vide\")}</div>);}"
                    );
            }
            return Ok("Fichier creer avec succès");
        }

        [HttpGet("creationDelete")]
        public async Task<IActionResult> creationDelete(string nomtable)
        {
            Console.WriteLine($"-----------generation crud Delete----------- path= {filePath}");

            filePath = filePath + "Supprimer_" + nomtable + ".jsx";
            string nomclasse = type.ToString().Split('.')[2].ToLower();
            string nomClasse = type.ToString().Split('.')[2];
            // Vérifiez si le fichier n'existe pas déjà
            if (System.IO.File.Exists(filePath))
            {
                return BadRequest("Le fichier existe deja");
            }

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                //ecriture import
                writer.WriteLine(
                    "import Apicontext from \"../../context/api-context\";" +
                    "import axios from \"axios\";" +
                    "import { useContext, useEffect, useState } from \"react\";" +
                    "import { useParams } from \"react-router-dom\";" +
                    "import httpErrorHandler from \"../HttpError\";"
                );
                //ecriture attribut
                writer.WriteLine(
                    "export default function Supprimer_" + nomtable + "() {" +
                    "const [message, setMessage] = useState(\"\");" +
                    "let { id } = useParams();" +
                    "const URL = useContext(Apicontext);" +
                    "const [" + nomclasse + ", set" + nomClasse + "] = useState({});");

                //ecriture fonction
                writer.WriteLine("//Les fonctions");

                writer.WriteLine(
                    "async function getData() {" +
                        "try {" +
                            "const url = `${ URL }/" + nomtable + "/${id}`; " +
                            "let response = await axios.get(url);" +
                            "response = response.data;" +
                            "set" + nomClasse + "(response);" +
                        "} catch (error) { setMessage(httpErrorHandler(error)); } " +
                    "}");

                writer.WriteLine("useEffect(() => { getData();}, [id]);");

                writer.WriteLine(
                    "const deleteData = async () => {" +
                        "try {" +
                            "const url = `${ URL }/" + nomtable + "/${id}`; " +
                            "let response = await axios.delete(url);" +
                            "if (response.status == 200) {" +
                                "setMessage(\"Success\");}" +
                        "} catch (error) { setMessage(httpErrorHandler(error)); }" +
                    "};");
                //affichage
                writer.WriteLine("return( " +
                                 "<div className=\"body-element\">" +
                                 "<div>" +
                                     "<label htmlFor=\"" + type.GetProperties()[0].Name.ToLower() + "\">" + type.GetProperties()[0].Name.ToLower() + "</label>" +
                                     "<input type=\"number\" id=\"" + type.GetProperties()[0].Name.ToLower() + "\" name=\"" + type.GetProperties()[0].Name.ToLower() + "\" value={" + nomclasse + "." + type.GetProperties()[0].Name.ToLower() + "} />" +
                                 "</div>");

                foreach (PropertyInfo property in type.GetProperties())
                {
                    writer.WriteLine("<label htmlFor=\"" + property.Name.ToLower() + "\" >" + property.Name + "</label>");
                    if (property.PropertyType == typeof(int) || Nullable.GetUnderlyingType(property.PropertyType) == typeof(int))
                    {
                        writer.WriteLine("<input type=\"number\" name=\"" + property.Name.ToLower() + "\" id=\"" + property.Name.ToLower() + "\" value={" + nomclasse + "." + property.Name.ToLower() + "} />");
                    }
                    else if (property.PropertyType == typeof(double) || Nullable.GetUnderlyingType(property.PropertyType) == typeof(double))
                    {
                        writer.WriteLine("<input step=\"0.01\" type=\"number\" name=\"" + property.Name.ToLower() + "\" id=\"" + property.Name.ToLower() + "\" value={" + nomclasse + "." + property.Name.ToLower() + "}  />");
                    }
                    else if (property.PropertyType == typeof(DateTime) || Nullable.GetUnderlyingType(property.PropertyType) == typeof(DateTime))
                    {
                        writer.WriteLine("<input type=\"datetime-local\" name=\"" + property.Name.ToLower() + "\" id=\"" + property.Name.ToLower() + "\" value={" + nomclasse + "." + property.Name.ToLower() + "} />");
                    }
                    else if (property.PropertyType == typeof(DateOnly) || Nullable.GetUnderlyingType(property.PropertyType) == typeof(DateOnly))
                    {
                        writer.WriteLine("<input type=\"date\" name=\"" + property.Name.ToLower() + "\" id=\"" + property.Name.ToLower() + "\" value={" + nomclasse + "." + property.Name.ToLower() + "} />");
                    }
                    else if (property.PropertyType == typeof(TimeOnly) || Nullable.GetUnderlyingType(property.PropertyType) == typeof(TimeOnly))
                    {
                        writer.WriteLine("<input type=\"time\" name=\"" + property.Name.ToLower() + "\" id=\"" + property.Name.ToLower() + "\" value={" + nomclasse + "." + property.Name.ToLower() + "} />");
                    }
                    else if (property.PropertyType == typeof(string) || Nullable.GetUnderlyingType(property.PropertyType) == typeof(string))
                    {
                        writer.WriteLine("<input type=\"text\" name=\"" + property.Name.ToLower() + "\" id=\"" + property.Name.ToLower() + "\" value={" + nomclasse + "." + property.Name.ToLower() + "} />");
                    }
                    else if (property.PropertyType == typeof(IFormFile) || Nullable.GetUnderlyingType(property.PropertyType) == typeof(IFormFile))
                    {
                        //writer.WriteLine("<input type=\"file\" name=\"" + property.Name.ToLower() + "\" id=\"" + property.Name.ToLower() + "\" value={" + nomclasse + "." + property.Name.ToLower() + "} />");
                        writer.WriteLine("<img src=\"\" alt=\"image_" + property.Name.ToLower() + "\" />");

                    }
                    else
                    {
                        writer.WriteLine(
                            "//attribut inconnue \n" +
                            "<input type=\"text\" name=\"" + property.Name.ToLower() + "\" id=\"" + property.Name.ToLower() + "\" value={" + nomclasse + "." + property.Name.ToLower() + "} />");
                    }
                }

                writer.WriteLine("{message && (<div className=\"error-message\">{JSON.stringify(message)}</div>)}");

                writer.WriteLine(
                    "<div className=\"div-validation\">" +
                        "<button className=\"boutton-annuler\">Annuler</button>" +
                        "<button className=\"boutton-valider\" onClick={deleteData}>Supprimer</button>" +
                    "</div>");
                writer.WriteLine("</div>");

                writer.WriteLine("); }");

            }
            return Ok("Fichier creer avec succès");

        }

        [HttpGet("creationAjout_FROMFORM")]
        public async Task<IActionResult> creationAjout_FORM(string nomtable)
        {
            Console.WriteLine($"-----------generation crud Ajout----------- path= {filePath}");

            filePath = filePath + "Ajout_" + nomtable + ".jsx";
            string nomclasse = type.ToString().Split('.')[2].ToLower();
            string nomClasse = type.ToString().Split('.')[2];
            // Vérifiez si le fichier n'existe pas déjà
            if (System.IO.File.Exists(filePath))
            {
                return BadRequest("Le fichier existe deja");
            }

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                //ecriture import
                writer.WriteLine(
                    "import Apicontext from \"../../context/api-context\";" +
                    "import axios from \"axios\";" +
                    "import { useContext, useEffect, useState } from \"react\";" +
                    "import JsonToForm from \"../../JsonToFrom\";" +
                    "import httpErrorHandler from \"../HttpError\";"
                );
                //ecriture attribut
                writer.WriteLine(
                    "export default function Ajout_" + nomtable + "() {" +
                    "const [message, setMessage] = useState(\"\");" +
                    "const URL = useContext(Apicontext);" +
                    "const [" + nomclasse + ", set" + nomClasse + "] = useState({});");

                //ecriture fonction
                writer.WriteLine("//Les fonctions");

                writer.WriteLine(
                    "const handleFileChange = (e) => {" +
                        "if (!e.target.files) {return;}" +
                        "set" + nomClasse + "({..." + nomclasse + ", [e.target.name]: e.target.files[0],});" +
                    "};");

                writer.WriteLine(
                    "const handleChange = (e) => {" +
                        "set" + nomClasse + "({..." + nomclasse + ",[e.target.name]: e.target.value,});setMessage(\"\");" +
                    "};");

                writer.WriteLine(
                    "const sendData = async () => {" +
                        "//multipart/form-data \n" +
                        "try {" +
                            "const url = `${ URL }/" + nomtable + "`; " +
                            "// Pour envoyer un fichier, vous devez utiliser FormData \n      " +
                            "const formData = JsonToForm(" + nomclasse + ");" +
                            "let response = await axios.post(url, " +
                                "formData, {headers: { \"Content-Type\": \"application/json\",},}" +
                            ");" +
                            "if (response.status == 200) {setMessage(\"Success\");}" +
                        "} catch (error) {setMessage(httpErrorHandler(error));}" +
                    "};");

                writer.WriteLine(
                    "return (" +
                        "<div className=\"body-element\">");
                foreach (PropertyInfo property in type.GetProperties())
                {
                    writer.WriteLine("<label htmlFor=\"" + property.Name.ToLower() + "\" >" + property.Name + "</label>");
                    if (property.PropertyType == typeof(int) || Nullable.GetUnderlyingType(property.PropertyType) == typeof(int))
                    {
                        writer.WriteLine("<input type=\"number\" name=\"" + property.Name.ToLower() + "\" id=\"" + property.Name.ToLower() + "\" onChange={handleChange} />");
                    }
                    else if (property.PropertyType == typeof(double) || Nullable.GetUnderlyingType(property.PropertyType) == typeof(double))
                    {
                        writer.WriteLine("<input step=\"0.01\" type=\"number\" name=\"" + property.Name.ToLower() + "\" id=\"" + property.Name.ToLower() + "\" onChange={handleChange}  />");
                    }
                    else if (property.PropertyType == typeof(DateTime) || Nullable.GetUnderlyingType(property.PropertyType) == typeof(DateTime))
                    {
                        writer.WriteLine("<input type=\"datetime-local\" name=\"" + property.Name.ToLower() + "\" id=\"" + property.Name.ToLower() + "\" onChange={handleChange} />");
                    }
                    else if (property.PropertyType == typeof(DateOnly) || Nullable.GetUnderlyingType(property.PropertyType) == typeof(DateOnly))
                    {
                        writer.WriteLine("<input type=\"date\" name=\"" + property.Name.ToLower() + "\" id=\"" + property.Name.ToLower() + "\" onChange={handleChange} />");
                    }
                    else if (property.PropertyType == typeof(TimeOnly) || Nullable.GetUnderlyingType(property.PropertyType) == typeof(TimeOnly))
                    {
                        writer.WriteLine("<input type=\"time\" name=\"" + property.Name.ToLower() + "\" id=\"" + property.Name.ToLower() + "\" onChange={handleChange} />");
                    }
                    else if (property.PropertyType == typeof(string) || Nullable.GetUnderlyingType(property.PropertyType) == typeof(string))
                    {
                        writer.WriteLine("<input type=\"text\" name=\"" + property.Name.ToLower() + "\" id=\"" + property.Name.ToLower() + "\" onChange={handleChange} />");
                    }
                    else if (property.PropertyType == typeof(IFormFile) || Nullable.GetUnderlyingType(property.PropertyType) == typeof(IFormFile))
                    {
                        writer.WriteLine("<input type=\"file\" name=\"" + property.Name.ToLower() + "\" id=\"" + property.Name.ToLower() + "\" onChange={handleFileChange} />");
                        //writer.WriteLine("<img src=\"\" alt=\"image_" + property.Name.ToLower() + "\" />");

                    }
                    else
                    {
                        writer.WriteLine(
                            "// attribut inconnue \n" +
                            "<input type=\"text\" name=\"" + property.Name.ToLower() + "\" id=\"" + property.Name.ToLower() + "\" onChange={handleChange} />");
                    }
                }
                writer.WriteLine(
                        "{message && (<div className=\"error-message\">{JSON.stringify(message)}</div>) }" +
                        "<div className=\"div-validation\">" +
                            "<button className=\"boutton-annuler\">Annuler</button>" +
                            "<button className=\"boutton-valider\" onClick={sendData}>Valider</button>" +
                        "</div>" +
                        "</div>" +
                    ");}");
            }

            return Ok("Fichier creer avec succès");
        }

        [HttpGet("creationAjout_FROMBODY")]
        public async Task<IActionResult> creationAjout_BODY(string nomtable)
        {
            Console.WriteLine($"-----------generation crud Ajout----------- path= {filePath}");

            filePath = filePath + "Ajout_" + nomtable + ".jsx";
            string nomclasse = type.ToString().Split('.')[2].ToLower();
            string nomClasse = type.ToString().Split('.')[2];
            // Vérifiez si le fichier n'existe pas déjà
            if (System.IO.File.Exists(filePath))
            {
                return BadRequest("Le fichier existe deja");
            }

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                //ecriture import
                writer.WriteLine(
                    "import Apicontext from \"../../context/api-context\";" +
                    "import axios from \"axios\";" +
                    "import JsonToForm from \"../../JsonToFrom\";" +
                    "import { useParams } from \"react-router-dom\";" +
                    "import { useContext, useEffect, useState } from \"react\";" +
                    "import httpErrorHandler from \"../HttpError\";"
                );
                //ecriture attribut
                writer.WriteLine(
                    "export default function Ajout_" + nomtable + "() {" +
                    "const [message, setMessage] = useState(\"\");" +
                    "const URL = useContext(Apicontext);" +
                    "const [" + nomclasse + ", set" + nomClasse + "] = useState({});");

                //ecriture fonction
                writer.WriteLine("//Les fonctions");
                writer.WriteLine(
                    "const handleChange = (e) => {" +
                        "set" + nomClasse + "({..." + nomclasse + ",[e.target.name]: e.target.value,});setMessage(\"\");" +
                    "};");

                writer.WriteLine(
                    "const sendData = async () => {" +
                        "try {" +
                            "const url = `${ URL }/" + nomtable + "`; " +
                            "let response = await axios.put(url, " + nomclasse + ");" +
                              "if (response.status == 200) {setMessage(\"Success\");}" +
                        "} catch (error) {setMessage(httpErrorHandler(error));}" +
                    "};");

                writer.WriteLine(
                    "return (" +
                        "<div className=\"body-element\">");
                foreach (PropertyInfo property in type.GetProperties())
                {
                    writer.WriteLine("<label htmlFor=\"" + property.Name.ToLower() + "\" >" + property.Name + "</label>");
                    if (property.PropertyType == typeof(int) || Nullable.GetUnderlyingType(property.PropertyType) == typeof(int))
                    {
                        writer.WriteLine("<input type=\"number\" name=\"" + property.Name.ToLower() + "\" id=\"" + property.Name.ToLower() + "\" onChange={handleChange} />");
                    }
                    else if (property.PropertyType == typeof(double) || Nullable.GetUnderlyingType(property.PropertyType) == typeof(double))
                    {
                        writer.WriteLine("<input step=\"0.01\" type=\"number\" name=\"" + property.Name.ToLower() + "\" id=\"" + property.Name.ToLower() + "\" onChange={handleChange}  />");
                    }
                    else if (property.PropertyType == typeof(DateTime) || Nullable.GetUnderlyingType(property.PropertyType) == typeof(DateTime))
                    {
                        writer.WriteLine("<input type=\"datetime-local\" name=\"" + property.Name.ToLower() + "\" id=\"" + property.Name.ToLower() + "\" onChange={handleChange} />");
                    }
                    else if (property.PropertyType == typeof(DateOnly) || Nullable.GetUnderlyingType(property.PropertyType) == typeof(DateOnly))
                    {
                        writer.WriteLine("<input type=\"date\" name=\"" + property.Name.ToLower() + "\" id=\"" + property.Name.ToLower() + "\" onChange={handleChange} />");
                    }
                    else if (property.PropertyType == typeof(TimeOnly) || Nullable.GetUnderlyingType(property.PropertyType) == typeof(TimeOnly))
                    {
                        writer.WriteLine("<input type=\"time\" name=\"" + property.Name.ToLower() + "\" id=\"" + property.Name.ToLower() + "\" onChange={handleChange} />");
                    }
                    else if (property.PropertyType == typeof(string) || Nullable.GetUnderlyingType(property.PropertyType) == typeof(string))
                    {
                        writer.WriteLine("<input type=\"text\" name=\"" + property.Name.ToLower() + "\" id=\"" + property.Name.ToLower() + "\" onChange={handleChange} />");
                    }
                    else
                    {
                        writer.WriteLine(
                            "// attribut inconnue \n" +
                            "<input type=\"text\" name=\"" + property.Name.ToLower() + "\" id=\"" + property.Name.ToLower() + "\" onChange={handleChange} />");
                    }


                }
                writer.WriteLine(
                    "{message && (<div className=\"error-message\">{JSON.stringify(message)}</div>) }" +
                    "<div className=\"div-validation\">" +
                    "<button className=\"boutton-annuler\">Annuler</button>" +
                    "<button className=\"boutton-valider\" onClick={sendData}>Valider</button>" +
                    "</div>" +
                    "</div>" +
                    ");}");
            }
            return Ok("Fichier creer avec succès");
        }

        [HttpGet("creationModifier_FROMFORM")]
        public async Task<IActionResult> creationModifier_FROMFORM(string nomtable)
        {

            Console.WriteLine($"-----------generation crud Modifier----------- path= {filePath}");

            filePath = filePath + "Modifier_" + nomtable + ".jsx";
            string nomclasse = type.ToString().Split('.')[2].ToLower();
            string nomClasse = type.ToString().Split('.')[2];
            // Vérifiez si le fichier n'existe pas déjà
            if (System.IO.File.Exists(filePath))
            {
                return BadRequest("Le fichier existe deja");
            }

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                //ecriture import
                writer.WriteLine(
                    "import Apicontext from \"../../context/api-context\";" +
                    "import axios from \"axios\";" +
                    "import JsonToForm from \"../../JsonToFrom\";" +
                    "import { useParams } from \"react-router-dom\";" +
                    "import { useContext, useEffect, useState } from \"react\";" +
                    "import httpErrorHandler from \"../HttpError\";"
                );

                //ecriture attribut
                writer.WriteLine(
                    "export default function Modifier_" + nomtable + "() {" +
                    "const [message, setMessage] = useState(\"\");" +
                    "let { id } = useParams();" +
                    "const URL = useContext(Apicontext);" +
                    "const [" + nomclasse + ", set" + nomClasse + "] = useState({});");

                //ecriture fonction
                writer.WriteLine("//Les fonctions");

                writer.WriteLine(
                    "async function getData() {" +
                        "try {" +
                            "const url = `${URL}/" + nomtable + "/${id}`;" +
                            "let response = await axios.get(url);" +
                            "response = response.data;" +
                            "set" + nomClasse + "(response);" +
                        "} catch (error) {setMessage(httpErrorHandler(error));}" +
                    "}");

                writer.WriteLine(
                    "const handleFileChange = (e) => {" +
                        "if (!e.target.files) {return;}" +
                        "set" + nomClasse + "({..." + nomclasse + ",image: e.target.files[0],});setMessage(\"\");" +
                    "};");

                writer.WriteLine("useEffect(() => {getData();}, [id]);");

                writer.WriteLine(
                    "const handleChange = (e) => {" +
                        "set" + nomClasse + "({..." + nomclasse + ",[e.target.name]: e.target.value});" +
                        "setMessage(\"\");" +
                    "};");

                writer.WriteLine(
                    "const sendData = async () => {" +
                        "//multipart/form-data \n" +
                        "try {" +
                            "const url = `${URL}/" + nomtable + "/${id}`;" +
                            "const formData = JsonToForm(" + nomclasse + ");" +
                            "let response = await axios.put(url, formData, {" +
                                "headers: {\"Content-Type\": \"application/json\",}," +
                            "});" +
                            "if (response.status == 200) {setMessage(\"Success\");}" +
                        "} catch (error) {setMessage(httpErrorHandler(error));}" +
                    "};");

                // affichage
                writer.WriteLine(
                    "return (" +
                        "<div className=\"body-element\">");

                foreach (PropertyInfo property in type.GetProperties())
                {
                    if (property == type.GetProperties()[0]) { writer.WriteLine("<div>"); }
                    writer.WriteLine("<label htmlFor=\"" + property.Name.ToLower() + "\" >" + property.Name + "</label>");
                    if (property.PropertyType == typeof(int) || Nullable.GetUnderlyingType(property.PropertyType) == typeof(int))
                    {
                        writer.WriteLine("<input type=\"number\" name=\"" + property.Name.ToLower() + "\" id=\"" + property.Name.ToLower() + "\" onChange={handleChange} defaultValue={" + nomclasse + "." + property.Name.ToLower() + "} />");
                    }
                    else if (property.PropertyType == typeof(double) || Nullable.GetUnderlyingType(property.PropertyType) == typeof(double))
                    {
                        writer.WriteLine("<input step=\"0.01\" type=\"number\" name=\"" + property.Name.ToLower() + "\" id=\"" + property.Name.ToLower() + "\" onChange={handleChange} defaultValue={" + nomclasse + "." + property.Name.ToLower() + "}  />");
                    }
                    else if (property.PropertyType == typeof(DateTime) || Nullable.GetUnderlyingType(property.PropertyType) == typeof(DateTime))
                    {
                        writer.WriteLine("<input type=\"datetime-local\" name=\"" + property.Name.ToLower() + "\" id=\"" + property.Name.ToLower() + "\" onChange={handleChange} defaultValue={" + nomclasse + "." + property.Name.ToLower() + "} />");
                    }
                    else if (property.PropertyType == typeof(DateOnly) || Nullable.GetUnderlyingType(property.PropertyType) == typeof(DateOnly))
                    {
                        writer.WriteLine("<input type=\"date\" name=\"" + property.Name.ToLower() + "\" id=\"" + property.Name.ToLower() + "\" onChange={handleChange} defaultValue={" + nomclasse + "." + property.Name.ToLower() + "} />");
                    }
                    else if (property.PropertyType == typeof(TimeOnly) || Nullable.GetUnderlyingType(property.PropertyType) == typeof(TimeOnly))
                    {
                        writer.WriteLine("<input type=\"time\" name=\"" + property.Name.ToLower() + "\" id=\"" + property.Name.ToLower() + "\" onChange={handleChange} defaultValue={" + nomclasse + "." + property.Name.ToLower() + "} />");
                    }
                    else if (property.PropertyType == typeof(string) || Nullable.GetUnderlyingType(property.PropertyType) == typeof(string))
                    {
                        writer.WriteLine("<input type=\"text\" name=\"" + property.Name.ToLower() + "\" id=\"" + property.Name.ToLower() + "\" onChange={handleChange} defaultValue={" + nomclasse + "." + property.Name.ToLower() + "} />");
                    }
                    else if (property.PropertyType == typeof(IFormFile) || Nullable.GetUnderlyingType(property.PropertyType) == typeof(IFormFile))
                    {
                        writer.WriteLine("<input type=\"file\" name=\"" + property.Name.ToLower() + "\" id=\"" + property.Name.ToLower() + "\" onChange={handleFileChange} defaultValue={" + nomclasse + "." + property.Name.ToLower() + "} />");
                        //writer.WriteLine("<img src=\"\" alt=\"image_" + property.Name.ToLower() + "\" />");

                    }
                    else
                    {
                        writer.WriteLine(
                            "// attribut inconnue \n" +
                            "<input type=\"text\" name=\"" + property.Name.ToLower() + "\" id=\"" + property.Name.ToLower() + "\" onChange={handleChange} defaultValue={" + nomclasse + "." + property.Name.ToLower() + "} />");
                    }
                    if (property == type.GetProperties()[0]) { writer.WriteLine("</div>"); }
                }

                writer.WriteLine(
                    "{message && (<div className=\"error-message\">{JSON.stringify(message)}</div>)}" +
                    "<div className=\"div-validation\">" +
                        "<button className=\"boutton-annuler\">Annuler</button>" +
                        "<button className=\"boutton-valider\" onClick={sendData}>Valider</button>" +
                    "</div>" +
                    "</div>" +
                    ");}");
            }
            return Ok("Fichier creer avec succès");
        }

        [HttpGet("creationModifier_FROMBODY")]
        public async Task<IActionResult> creationModifier_BODY(string nomtable)
        {

            Console.WriteLine($"-----------generation crud Modifier----------- path= {filePath}");

            filePath = filePath + "Modifier_" + nomtable + ".jsx";
            string nomclasse = type.ToString().Split('.')[2].ToLower();
            string nomClasse = type.ToString().Split('.')[2];
            // Vérifiez si le fichier n'existe pas déjà
            if (System.IO.File.Exists(filePath))
            {
                return BadRequest("Le fichier existe deja");
            }

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                //ecriture import
                writer.WriteLine(
                    "import Apicontext from \"../../context/api-context\";" +
                    "import axios from \"axios\";" +
                    "import { useContext, useEffect, useState } from \"react\";" +
                    "import httpErrorHandler from \"../HttpError\";" +
                    "import { useParams } from \"react-router-dom\";"
                );

                //ecriture attribut
                writer.WriteLine(
                    "export default function Modifier_" + nomtable + "() {" +
                    "const [message, setMessage] = useState(\"\");" +
                    "let { id } = useParams();" +
                    "const URL = useContext(Apicontext);" +
                    "const [" + nomclasse + ", set" + nomClasse + "] = useState({});");

                //ecriture fonction
                writer.WriteLine("//Les fonctions");
                writer.WriteLine(
                    "async function getData() {" +
                        "try {" +
                            "const url = `${URL}/" + nomtable + "/${id}`;" +
                            "let response = await axios.get(url);" +
                            "response = response.data;" +
                            "set" + nomClasse + "(response);" +
                        "} catch (error) {setMessage(httpErrorHandler(error));}" +
                    "}");

                writer.WriteLine("useEffect(() => {getData();}, [id]);");

                writer.WriteLine(
                    "const handleChange = (e) => {" +
                        "set" + nomClasse + "({..." + nomclasse + ",[e.target.name]: e.target.value});" +
                        "setMessage(\"\");" +
                    "};");

                writer.WriteLine(
                    "const sendData = async () => {" +
                        "try {" +
                            "const url = `${URL}/" + nomtable + "/${id}`;" +
                            "let response = await axios.put(url," + nomClasse + ");" +
                            "if (response.status == 200) {setMessage(\"Success\");}" +
                        "} catch (error) {setMessage(httpErrorHandler(error));}" +
                    "};");

                // affichage
                writer.WriteLine(
                    "return (" +
                        "<div className=\"body-element\">");

                foreach (PropertyInfo property in type.GetProperties())
                {
                    if (property == type.GetProperties()[0]) { writer.WriteLine("<div>"); }
                    writer.WriteLine("<label htmlFor=\"" + property.Name.ToLower() + "\" >" + property.Name + "</label>");
                    if (property.PropertyType == typeof(int) || Nullable.GetUnderlyingType(property.PropertyType) == typeof(int))
                    {
                        writer.WriteLine("<input type=\"number\" name=\"" + property.Name.ToLower() + "\" id=\"" + property.Name.ToLower() + "\" onChange={handleChange} defaultValue={" + nomclasse + "." + property.Name.ToLower() + "} />");
                    }
                    else if (property.PropertyType == typeof(double) || Nullable.GetUnderlyingType(property.PropertyType) == typeof(double))
                    {
                        writer.WriteLine("<input step=\"0.01\" type=\"number\" name=\"" + property.Name.ToLower() + "\" id=\"" + property.Name.ToLower() + "\" onChange={handleChange} defaultValue={" + nomclasse + "." + property.Name.ToLower() + "}  />");
                    }
                    else if (property.PropertyType == typeof(DateTime) || Nullable.GetUnderlyingType(property.PropertyType) == typeof(DateTime))
                    {
                        writer.WriteLine("<input type=\"datetime-local\" name=\"" + property.Name.ToLower() + "\" id=\"" + property.Name.ToLower() + "\" onChange={handleChange} defaultValue={" + nomclasse + "." + property.Name.ToLower() + "} />");
                    }
                    else if (property.PropertyType == typeof(DateTimeOffset) || Nullable.GetUnderlyingType(property.PropertyType) == typeof(DateTimeOffset))
                    {
                        writer.WriteLine("<input type=\"datetime-local\" name=\"" + property.Name.ToLower() + "\" id=\"" + property.Name.ToLower() + "\" onChange={handleChange} defaultValue={" + nomclasse + "." + property.Name.ToLower() + "} />");
                    }
                    else if (property.PropertyType == typeof(DateOnly) || Nullable.GetUnderlyingType(property.PropertyType) == typeof(DateOnly))
                    {
                        writer.WriteLine("<input type=\"date\" name=\"" + property.Name.ToLower() + "\" id=\"" + property.Name.ToLower() + "\" onChange={handleChange} defaultValue={" + nomclasse + "." + property.Name.ToLower() + "} />");
                    }
                    else if (property.PropertyType == typeof(TimeOnly) || Nullable.GetUnderlyingType(property.PropertyType) == typeof(TimeOnly))
                    {
                        writer.WriteLine("<input type=\"time\" name=\"" + property.Name.ToLower() + "\" id=\"" + property.Name.ToLower() + "\" onChange={handleChange} defaultValue={" + nomclasse + "." + property.Name.ToLower() + "} />");
                    }
                    else if (property.PropertyType == typeof(string) || Nullable.GetUnderlyingType(property.PropertyType) == typeof(string))
                    {
                        writer.WriteLine("<input type=\"text\" name=\"" + property.Name.ToLower() + "\" id=\"" + property.Name.ToLower() + "\" onChange={handleChange} defaultValue={" + nomclasse + "." + property.Name.ToLower() + "} />");
                    }
                    else
                    {
                        writer.WriteLine(
                            "// attribut inconnue \n" +
                            "<input type=\"text\" name=\"" + property.Name.ToLower() + "\" id=\"" + property.Name.ToLower() + "\" onChange={handleChange} defaultValue={" + nomclasse + "." + property.Name.ToLower() + "} />");
                    }
                    if (property == type.GetProperties()[0]) { writer.WriteLine("</div>"); }
                }

                writer.WriteLine(
                    "{message && (<div className=\"error-message\">{JSON.stringify(message)}</div>)}" +
                    "<div className=\"div-validation\">" +
                        "<button className=\"boutton-annuler\">Annuler</button>" +
                        "<button className=\"boutton-valider\" onClick={sendData}>Valider</button>" +
                    "</div>" +
                    "</div>" +
                    ");}");
            }
            return Ok("Fichier creer avec succès");
        }

        [HttpGet("creationCRUD")]
        public async Task<IActionResult> CreationCRUD(string nomtable)
        {
            var Creer = creationAjout_FORM(nomtable);
            var Supprimer = creationDelete(nomtable);
            var Liste = creationListPagination(nomtable);
            var Modifier = creationModifier_FROMFORM(nomtable);
            Dictionary<string, Task<IActionResult>> rep = new Dictionary<string, Task<IActionResult>>()
            {
                { "Creer", Creer },
                { "Supprimer", Supprimer },
                { "Liste", Liste },
                { "Modifier", Modifier }

            };
            return Ok(rep);
        }

    }
}
