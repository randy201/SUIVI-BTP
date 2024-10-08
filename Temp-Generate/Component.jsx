
const image = () => {
    <img
     style={{ width: "50px", height: "50px" }}
     src={"/src/assets/" + unite.image}
     alt="image_photo_profil"
    />
}

const getData = () => {
  const [unite, setUnite] = useState([]);

    async function getData() {
        try {
            const url = `${URL}/Unites`;
            let response = await axios.get(url);
            response = response.data;
            setUnite(response);
        } catch (error) {
            setMessage(httpErrorHandler(error));
        }
    }

    useEffect(() => {
        getData();
    }, []);
}

const select = () => {
    function changeSelect(e){
        const selectedPersonne = listElement.find((element) => element.id === parseInt(e.target.value));
        setPersonne(selectedPersonne);
    }

    const handlechangeSelect = (e) => {
        setPersonne({
            ...personne,
            profil: { id: e.target.value },
        });
    };
    return (<>
        <select defaultValue={Unite.id} onChange={changeSelect}>
            {listElement.map((element) => (
                <option key={element.id} value={element.id}>
                    {element.image}
                </option>
            ))}
        </select>
    </>)
}

const check_box = () => {
    const [unite, setUnite] = useState([]);
    const [checkedValues, setCheckedValues] = useState([]);
    
    const handleCheckboxChange = (event) => {
        const { value, checked } = event.target;
        if (checked) {
          // Si la case est cochée, ajouter la valeur à l'état
          setCheckedValues([...checkedValues, value]);
        } else {
          // Si la case est décochée, retirer la valeur de l'état
          setCheckedValues(checkedValues.filter((val) => val !== value));
        }
      };

    return (<>

        <label htmlFor="unite">Unite</label>
          <div className="grid-container">
            {place.map((obj) => (
              <div key={obj.id_unite} className="grid-item">
                <input
                  type="checkbox"
                  name="id_unite"
                  id={obj.id_unite}
                  value={obj.id_unite}
                  defaultChecked={[58, 67, 62].includes(obj.id_unite)}
                  onChange={handleCheckboxChange}
                />
                <label htmlFor={obj.id_unite}>{obj.nom}</label>
              </div>
            ))}
          </div>

      </>)
}

const radio = () => {
    const [unite, setUnite] = useState([]);

    async function getData() {
        try {
            const url = `${URL}/Unites`;
            let response = await axios.get(url);
            response = response.data;
            setUnite(response);
        } catch (error) {
            setMessage(httpErrorHandler(error));
        }
    }

    useEffect(() => {
        getData();
    }, []);

    return (<>

        <label htmlFor="">Unite</label>
        <div className="grid-container">
        {unite.map((obj) => (
            <div key={obj.id_unite} className="grid-item">
              
                  <input
                    type="radio"
                    name="id_unite"
                    id={obj.id_unite}
                    defaultChecked={obj.id_unite === 1}
                    value={obj.id_unite}

                  />
                  <label htmlFor={obj.id_unite} >
                    {obj.nom}
                  </label>
            </div>
          ))}
        </div>

    </>
}

///Json no alefa

let response = await axios.post(url, formData, {
    headers: {
        "Content-Type": "application/json",
    },
});

//Beare
axios.defaults.headers.common[
        "Authorization"
      ] = `Bearer ${localStorage.getItem("tokken")}`;
      

//Send objet
const sendData = async () => {
    const data = {
      quantite: 0, // Par exemple, 0 comme quantité (à adapter à vos besoins)
      nom: "huhu", // Utiliser les valeurs des cases cochées
    };
    try {
      const url = `${URL}/Unites`;

      let response = await axios.post(url, data);
      if (response.status == 200) {
        setMessage("Success");
      }
    } catch (error) {
      setMessage(httpErrorHandler(error));
    }
  };