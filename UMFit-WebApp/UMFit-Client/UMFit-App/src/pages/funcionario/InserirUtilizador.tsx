
import { IonButton, IonCol, IonContent, IonDatetime, IonGrid, IonHeader, IonIcon, IonInput, IonItem, IonLabel, IonLoading, IonPage, IonRow, IonSelect, IonSelectOption, IonText, IonTitle, IonToolbar, IonButtons, IonMenuButton } from "@ionic/react";
import sha256 from "fast-sha256";
import { calendarOutline, cardOutline, codeWorkingOutline, locationOutline, mailOutline, peopleCircleOutline, personOutline, transgenderOutline } from "ionicons/icons";
import React from "react";
import { createUserAPI } from "../../models/API/UserAPI";
import { getTestValueUser, User } from "../../models/Other/User";
import "../css/InserirUtilizador.css";


//---------------------------------------------------------------------------------------------------------------------------------

class InserirUtilizador extends React.Component<any> {

    state: {
        nome_completo: string,
        email: string,
        password: string,
        localidade: string,
        nif: string,
        tipoDeSocio: string,
        genero: string,
        data_nascimento: string,
        categoria: string,
        loadingAPI: false,
        overflow : string,
        popup : string
    }

    stateToAPI: {
        newUser: User,
        passwordHash: string                
    }

    constructor(props: any) {

        super(props);

        this.state = {
            nome_completo: "",
            email: "",
            password: "",
            localidade: "",
            nif: "",
            tipoDeSocio: "",
            genero: "",
            data_nascimento: "1999-02-20",
            categoria: "",
            loadingAPI: false,
            overflow : "",
            popup:""
        }

        this.stateToAPI = {
            newUser: getTestValueUser(),
            passwordHash: ""
        }  
    }

    createUserState() {

        this.setState({
            loadingAPI: true
        });

        var genero = -2;
        switch(this.state.genero) {
            case "Masculino": genero = 1; break; 
            case "Feminino":  genero = 0; break; 
            default:          genero = -1; break; 
        }

        var categoria = "N??o tem";
        var socio = this.state.tipoDeSocio;
        if (this.state.tipoDeSocio === "Cliente Premium") {
            categoria = "Premium";
            socio = "Cliente";
        } else if (this.state.tipoDeSocio === "Cliente Standard") {
            categoria = "Standard";
            socio = "Cliente";
        }

        var user = new User(
            socio,
            this.state.email,
            parseInt(this.state.nif),
            this.state.nome_completo,
            genero,
            this.state.data_nascimento,
            this.state.localidade,
            categoria            
        );

        let pass_enc = new TextEncoder();
        let encoded = pass_enc.encode(this.state.password);
        let hash256 = Buffer.from(sha256(encoded)).toString('hex').toUpperCase();

        this.stateToAPI = {
            newUser: user,
            passwordHash: hash256
        }   

        createUserAPI(this.stateToAPI).then(
            
            res => {
                if (res.status===200 )alert("Criou um utilizador com sucesso!"); else return alert("Credenciais Invalidas");    
                res.json()}            
        ).then(
        
            (jsonData) => {
                
                this.setState({
                    loadingAPI: false
                });
        
                console.log("Got json:");
                console.log(jsonData);
                
            }
            
        )
        .finally(()=>{window.location.reload();}
        );
        
        
        }

    clearState() {
        this.setState({
            nome_completo: "",
            email: "",
            password: "",
            localidade: "",
            nif: "",
            tipoDeSocio: "",
            genero: "",
            data_nascimento: "1999-02-20"
        });        
    }

    render() {
        return(
            
            <IonPage>

                <IonHeader>
                <IonToolbar color="primary">
                <IonButtons slot="start">
                    <IonMenuButton />
                </IonButtons>
                <IonTitle id="page-title">Novo utilizador</IonTitle>
                </IonToolbar>
                </IonHeader>
    
                <IonContent className="PageContent">

                    <IonGrid className="PageGrid">

                        <IonRow>
                            <IonCol size="2.5" className="Row1ProfilePic">
                                
                                <img className="profilePicture" src={require('../../imgs/perfil_pic.png')} alt="Loading..."/> 
                            
                            </IonCol>

                            <IonCol>

                                <IonGrid>
                                
                                <div className="separador"></div>
                                    <IonRow>

                                        <IonCol className="FirstForm">

                                            <IonItem>
                                                <IonIcon slot="start" icon={personOutline}></IonIcon>
                                              <IonLabel position="floating">Nome Completo   
                                                    <IonText color="danger">*</IonText>   
        <IonText color={ (parseInt(this.state.overflow.split('/',1)[0])>=20)? "danger": "secondary"} >{this.state.overflow.length>0? '  ('+ this.state.overflow + ')' : "" }</IonText>
                                              </IonLabel>
                                                <IonInput required value={this.state.nome_completo} onIonChange={(e : any) => {
                                                    var ret :string = e.detail.value!
                                                    ret=ret.split(" ",1)[0]
                                                    this.setState({ overflow : ret.length>0? + ret.length+"/20" :"",nome_completo: e.detail.value! })
                                                }}></IonInput>
                                            </IonItem>                                           

                                            <IonItem>
                                                <IonIcon slot="start" icon={mailOutline}></IonIcon>
                                                <IonLabel position="floating">E-Mail <IonText color="danger">*</IonText></IonLabel>
                                                <IonInput required type="email" value={this.state.email} onIonChange={(e) => {
                                                    this.setState({ email: (e.target as HTMLInputElement).value });
                                                }}></IonInput>
                                            </IonItem>                                           

                                            <IonItem>
                                                <IonIcon slot="start" icon={codeWorkingOutline}></IonIcon>
                                                <IonLabel position="floating">Definir a password <IonText color="danger">*</IonText></IonLabel>
                                                <IonInput type="password" required value={this.state.password} onIonChange={(e) => {
                                                    this.setState({ password: (e.target as HTMLInputElement).value });
                                                }}></IonInput>
                                            </IonItem>                                           
 
                                            <IonItem>
                                                <IonIcon slot="start" icon={locationOutline}></IonIcon>
                                                <IonLabel position="floating">Localidade</IonLabel>
                                                <IonInput value={this.state.localidade} onIonChange={(e) => {
                                                    this.setState({ localidade: (e.target as HTMLInputElement).value });
                                                }}></IonInput>
                                            </IonItem>                                           

                                            <IonItem>
                                                <IonIcon slot="start" icon={cardOutline}></IonIcon>
                                                <IonLabel position="floating">Nif</IonLabel>
                                                <IonInput required pattern="[0-9]*" type="number" value={this.state.nif} onIonChange={(e) => {
                                                    this.setState({ nif: (e.target as HTMLInputElement).value });
                                                }}></IonInput>
                                            </IonItem>                                           
                                           
                                        </IonCol>

                                    </IonRow>

                                    <div className="separador"></div>
                                    <IonRow>
                                        <IonCol>

                                            <IonItem className="SelectUser">
                                                <IonIcon slot="start" icon={peopleCircleOutline}></IonIcon>
                                                <IonLabel>Tipo de s??cio: </IonLabel>
                                                <IonSelect value={this.state.tipoDeSocio} onIonChange={(e) => this.setState({ tipoDeSocio: e.detail.value! })}>
                                                    <IonSelectOption value="Cliente Standard">Cliente Standard</IonSelectOption>
                                                    <IonSelectOption value="Cliente Premium">Cliente Premium</IonSelectOption>
                                                    <IonSelectOption value="Instrutor">Instrutor</IonSelectOption>
                                                    <IonSelectOption value="Rececionista">Rececionista</IonSelectOption>
                                                </IonSelect>
                                            </IonItem>

                                            <IonItem className="SelectGender">
                                                <IonIcon slot="start" icon={transgenderOutline}></IonIcon>
                                                <IonLabel>G??nero: </IonLabel>
                                                <IonSelect value={this.state.genero} onIonChange={(e) => this.setState({ genero: e.detail.value! })}>
                                                    <IonSelectOption value="Masculino">Masculino</IonSelectOption>
                                                    <IonSelectOption value="Feminino">Feminino</IonSelectOption>
                                                    <IonSelectOption value="N??o especificar">N??o especificar</IonSelectOption>
                                                </IonSelect>
                                            </IonItem>
                                            
                                            <IonItem className="SelectDate">
                                                <IonIcon slot="start" icon={calendarOutline}></IonIcon>
                                                <IonLabel><div className="textResponsive">Data de Nasc.:</div></IonLabel>
                                                <IonDatetime value={this.state.data_nascimento} onIonChange={(e) => {this.setState({ data_nascimento: e.detail.value! })}}></IonDatetime>

                                            </IonItem>

                                        </IonCol>

                                    </IonRow>

                                    <div className="separador"></div>

                                    <IonRow className="buttonWrapper">

                                        <IonCol>
                                            <IonButton className="submitUser" expand="block" color="success" onClick={(event) => {
                                                event.preventDefault();
                                                this.createUserState();

                                            }}>Criar novo utilizador</IonButton>
                                        </IonCol>

                                        <IonCol>
                                            <IonButton className="clearForm" expand="block" color="light" onClick={() => {
                                                this.clearState();
                                            }}>Limpar formul??rio</IonButton>
                                        </IonCol>

                                    </IonRow>

                                </IonGrid>

                            </IonCol>
                        </IonRow>

                    </IonGrid>

                    <IonLoading
                        isOpen={this.state.loadingAPI}
                        message={'Please wait...'}
                        />

                </IonContent>
                
            </IonPage>    
        );
    }
}

export default InserirUtilizador;

    /*
        (nome + , email + , nif + , data_nasc, genero, categoria, localidade + ), 
        tipo (0:cliente, 1:instrutor, 2:rececionista)
        hash password
    */
