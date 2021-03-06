import IP_ADDR from "./API_DEFAULTS";

export {}

var baseURL: string = "https://" + IP_ADDR + "/api/planotreino";

export async function getListaExercicios () {

    const res = fetch(baseURL + "/exercicios", {
        method: 'post',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
            valueST: localStorage.getItem("token")
        })
    });
    

    return res;
}

export async function setPlanoTreino( pt:any ){

    const res = fetch(baseURL , {
        method: 'post',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        },
        body: JSON.stringify( pt)
    });
    

    return res;
}

export async function getPlanosTreino(mail_cliente: string){

    const res = fetch(baseURL + "/consultar", {
        method: 'post',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
            email: mail_cliente,
            valueST: localStorage.getItem("token")
        })
    });

    return res;
}