
import { IonCard, IonCardHeader, IonCardTitle, IonCol, IonContent, IonGrid, IonHeader, IonItem, IonList, IonPage, IonRow, IonSelect, IonSelectOption, IonTitle, IonToolbar } from "@ionic/react";
import React from "react";
import { Line } from "react-chartjs-2";
import "../css/Evolucao.css";


interface registoAvaliacao {
    data: Date,
    y: number
}

class EvolucaoSAVE extends React.Component {

    state: {
        peso: Array<registoAvaliacao>
        imc: Array<registoAvaliacao>
        mGorda: Array<registoAvaliacao>
        mMagra: Array<registoAvaliacao>
        idadeMeta: Array<registoAvaliacao>
        menuOpt:String
    }

    constructor(props: any) {

        super(props);   
            
        this.state = ({

            peso: new Array<registoAvaliacao>(),
            imc: new Array<registoAvaliacao>(),
            mGorda: new Array<registoAvaliacao>(),
            mMagra: new Array<registoAvaliacao>(),
            idadeMeta: new Array<registoAvaliacao>(),
            menuOpt: "compoCorporalOption" //default option

        });
    }

    componentDidMount() {

        this.setState({
            peso: [
                {data: new Date(2020, 0, 21), y: 98},
                {data: new Date(2020, 3, 2), y: 95},
                {data: new Date(2020, 5, 23), y: 93},
                {data: new Date(2020, 7, 12), y: 88},
                {data: new Date(2020, 10, 18), y: 82}
            ],
            imc: [
                {data: new Date(2020, 0, 21), y: 8},
                {data: new Date(2020, 3, 2), y: 5},
                {data: new Date(2020, 5, 23), y: 3},
                {data: new Date(2020, 7, 12), y: 2.3},
                {data: new Date(2020, 10, 18), y: 2.1}
            ],
            mGorda: [
                {data: new Date(2020, 0, 21), y: 50},
                {data: new Date(2020, 3, 2), y: 47},
                {data: new Date(2020, 5, 23), y: 42},
                {data: new Date(2020, 7, 12), y: 38},
                {data: new Date(2020, 10, 18), y: 27}
            ],
            mMagra: [
                {data: new Date(2020, 0, 21), y: 23},
                {data: new Date(2020, 3, 2), y: 25},
                {data: new Date(2020, 5, 23), y: 33},
                {data: new Date(2020, 7, 12), y: 36},
                {data: new Date(2020, 10, 18), y: 39}
            ],
            idadeMeta: [
                {data: new Date(2020, 0, 21), y: 269},
                {data: new Date(2020, 3, 2), y: 112},
                {data: new Date(2020, 5, 23), y: 82},
                {data: new Date(2020, 7, 12), y: 69},
                {data: new Date(2020, 10, 18), y: 12}
            ]
        });
    }

    render() {
        
        const geral = { 
            labels: [],
            datasets: [{ 
                fill: false, lineTension: 0.1,
                backgroundColor: 'rgba(75,192,192,0.4)', borderColor: 'rgba(75,192,192,1)',
                borderCapStyle: 'butt', borderDash: [], borderDashOffset: 0.0, borderJoinStyle: 'miter',
                pointBorderColor: 'rgba(75,192,192,1)', pointBackgroundColor: '#fff',
                pointBorderWidth: 1, pointHoverRadius: 5,
                pointHoverBackgroundColor: 'rgba(75,192,192,1)', pointHoverBorderColor: 'rgba(220,220,220,1)',
                pointHoverBorderWidth: 2, pointRadius: 1, pointHitRadius: 10, 
                data: [] 
            }]};

        //----------------------------------------------------------------------
        //Defini????o para todos os gr??ficos

        const noLegend={ legend: { display: false } }; 

        //----------------------------------------------------------------------
        //Def. indiviual dos dados de cada gr??fico
        //----------------------------------------------------------------------

        //----------------------------------------------------------------------
        //Compos. corporal => Graph. Peso (kg) 
        var xPeso: any[] = [], yPeso: number[] = [];
        const peso = this.state.peso;
        peso.forEach((ref)=>{
            xPeso.push(""+(ref.data.getMonth()+1)+"/"+ref.data.getDay());
            yPeso.push(ref.y);
        });
        const dataPeso = { 
            labels: xPeso,
            datasets: [{ 
                fill: false, lineTension: 0.1,
                backgroundColor: 'rgba(75,192,192,0.4)', borderColor: 'rgba(75,192,192,1)',
                borderCapStyle: 'butt', borderDash: [], borderDashOffset: 0.0, borderJoinStyle: 'miter',
                pointBorderColor: 'rgba(75,192,192,1)', pointBackgroundColor: '#fff',
                pointBorderWidth: 1, pointHoverRadius: 5,
                pointHoverBackgroundColor: 'rgba(75,192,192,1)', pointHoverBorderColor: 'rgba(220,220,220,1)',
                pointHoverBorderWidth: 2, pointRadius: 1, pointHitRadius: 10, 
                data: yPeso 
            }]};

        //----------------------------------------------------------------------
        //Compos. corporal => Graph. Massa Gorda (kg)

        var xMGorda : any[] = [], yMGorda: number[] = [];
        const mgorda = this.state.mGorda;
        mgorda.forEach((ref)=>{xMGorda.push(""+(ref.data.getMonth()+1)+"/"+ref.data.getDay());yMGorda.push(ref.y);}); 
        const dataMGorda = { 
            labels: xPeso,
            datasets: [{ 
                fill: false, lineTension: 0.1,
                backgroundColor: 'rgba(75,192,192,0.4)', borderColor: 'rgba(75,192,192,1)',
                borderCapStyle: 'butt', borderDash: [], borderDashOffset: 0.0, borderJoinStyle: 'miter',
                pointBorderColor: 'rgba(75,192,192,1)', pointBackgroundColor: '#fff',
                pointBorderWidth: 1, pointHoverRadius: 5,
                pointHoverBackgroundColor: 'rgba(75,192,192,1)', pointHoverBorderColor: 'rgba(220,220,220,1)',
                pointHoverBorderWidth: 2, pointRadius: 1, pointHitRadius: 10, 
                data: yMGorda 
            }]};

        var menuOp = this.state.menuOpt;
        
        if(this.state.menuOpt !== menuOp) {

            this.setState( { menuOpt: menuOp } )
        } 


        return(

            <IonPage>
            
                {/* Page title */}

                <IonHeader>
                    <IonToolbar color="primary">
                        <IonTitle id="page-title">Evolu????o</IonTitle>
                    </IonToolbar>
                </IonHeader>
            
                {/* Menu superior para escolha de grupo de gr??ficos */}
            
                <IonList>
                    
                    {/*
                    <IonListHeader>
                        <IonLabel></IonLabel>
                    </IonListHeader>
                    */}

                    <IonItem className="SelectionMenuItem">
    
                            <IonSelect value={this.state.menuOpt} placeholder="Select One" onIonChange={
                                e =>this.setState( {menuOpt:e.detail.value})
                            }>

                                <IonSelectOption value="compoCorporalOption"><div className="SMlabel">Composi????o corporal</div></IonSelectOption>
                                <IonSelectOption value="perimCorporalOption"><div className="SMlabel">Per??metros corporais</div></IonSelectOption>

                            </IonSelect>
                    </IonItem>

                </IonList>

                {/* Gr??ficos disponiveis */}

                <IonContent className="ionContent">
                
                <IonGrid className="grid">

                    <IonRow class="ion-justify-content-center">
                    
                        {( () => {
                            
                            //Apresentar diferentes menus mediante a sele????o
                            switch(this.state.menuOpt) {
                                
                                //Caso a sele????o de gr??ficos seja composi????o corporal
                                case "compoCorporalOption": 
                                
                                    return(

                                        <React.Fragment>
                                            
                                            {/* Gr??fico de Evolu????o do Peso */}

                                            <IonCol sizeXs="12" size-Sm ="12" sizeMd="6" sizeLg="6">

                                                <IonCard className="titleCard">

                                                    <IonCardHeader>
                                                        <IonCardTitle><div className="textWrapper">Peso</div></IonCardTitle>
                                                    </IonCardHeader>

                                                </IonCard>

                                                <div className="evoStats">
                                                    <Line ref="lineChart" data={dataPeso} options={noLegend} />
                                                </div>

                                            </IonCol>
                                            
                                            {/* Gr??fico de Evolu????o de Massa Gorda (Kg) */}

                                            <IonCol sizeXs="12" size-Sm ="12" sizeMd="6" sizeLg="6">

                                                <IonCard className="titleCard">

                                                    <IonCardHeader>
                                                        <IonCardTitle><div className="textWrapper">Massa Gorda (Kg)</div></IonCardTitle>
                                                    </IonCardHeader>

                                                </IonCard>

                                            <div className="evoStats">
                                                <Line ref="lineChart" data={dataMGorda} options={noLegend} />
                                            </div>

                                            </IonCol>

                                            {/* Gr??fico de Evolu????o de Massa Magra (Kg) */}

                                            <IonCol sizeXs="12" size-Sm ="12" sizeMd="6" sizeLg="6">

                                                <IonCard className="titleCard">

                                                    <IonCardHeader>
                                                        <IonCardTitle><div className="textWrapper">Massa Gorda (Kg)</div></IonCardTitle>
                                                    </IonCardHeader>

                                                </IonCard>

                                            <div className="evoStats">
                                                <Line ref="lineChart" data={dataMGorda} options={noLegend} />
                                            </div>

                                            </IonCol>

                                            {/* Gr??fico de Evolu????o de IMC */}

                                            <IonCol sizeXs="12" size-Sm ="12" sizeMd="6" sizeLg="6">

                                                <IonCard className="titleCard">

                                                    <IonCardHeader>
                                                        <IonCardTitle><div className="textWrapper">Massa Gorda (Kg)</div></IonCardTitle>
                                                    </IonCardHeader>

                                                </IonCard>

                                            <div className="evoStats">
                                                <Line ref="lineChart" data={dataMGorda} options={noLegend} />
                                            </div>

                                            </IonCol>

                                            {/* Gr??fico de Evolu????o de Massa Gorda (Kg) */}

                                            <IonCol sizeXs="12" size-Sm ="12" sizeMd="6" sizeLg="6">

                                                <IonCard className="titleCard">

                                                    <IonCardHeader>
                                                        <IonCardTitle><div className="textWrapper">Massa Gorda (Kg)</div></IonCardTitle>
                                                    </IonCardHeader>

                                                </IonCard>

                                            <div className="evoStats">
                                                <Line ref="lineChart" data={dataMGorda} options={noLegend} />
                                            </div>

                                            </IonCol>

                                        </React.Fragment>
                                    );

                                //Caso a sele????o de gr??ficos seja composi????o corporal
                                case "perimCorporalOption": 
                                
                                    return(

                                        <React.Fragment>
                                            
                                            {/* Gr??fico de Evolu????o do Peso */}
                                            <IonCol sizeXs="12" size-Sm ="12" sizeMd="6" sizeLg="6">

                                                <IonCard className="titleCard">

                                                    <IonCardHeader>
                                                        <IonCardTitle><div className="textWrapper">Bra??o direito</div></IonCardTitle>
                                                    </IonCardHeader>

                                                </IonCard>

                                                <div className="evoStats">
                                                    <Line ref="lineChart" data={dataPeso} options={noLegend} />
                                                </div>

                                            </IonCol>
                                            
                                            {/* Gr??fico de Evolu????o de Massa Gorda (Kg) */}

                                            <IonCol sizeXs="12" size-Sm ="12" sizeMd="6" sizeLg="6">

                                                <IonCard className="titleCard">

                                                    <IonCardHeader>
                                                        <IonCardTitle><div className="textWrapper">Bra??o Esquerdo</div></IonCardTitle>
                                                    </IonCardHeader>

                                                </IonCard>

                                            <div className="evoStats">
                                                <Line ref="lineChart" data={dataMGorda} options={noLegend} />
                                            </div>

                                            </IonCol>

                                        </React.Fragment>
                                    );

                                    default:
                                        
                                        return( 
                                            <IonCol sizeXs="12" size-Sm ="12" sizeMd="6" sizeLg="6">
                                            {/* Empty option */}
                                            </IonCol>
                                        );

                            } //close switch

                        } //close anonymouse function
                    ) ()}

                    </IonRow>

                </IonGrid>

            </IonContent>


        </IonPage>
        );
    }

}

export default EvolucaoSAVE;