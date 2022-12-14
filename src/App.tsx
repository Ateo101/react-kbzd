import React, {useState} from 'react';
import Accordion from "./components/Accordion/Accordion";
import Rating from "./components/Rating/Rating";
import OnOff from "./components/OnOff/OnOff";
import UncontrolledOnOff from "./components/OnOff/UncontrolledOnOff";
import UncontrolledAccordion from "./components/Accordion/UncontrolledAccordion";
import UncontrolledRating from "./components/Rating/UncontrolledRating";
import s from './App.module.css'
import Select from "./components/Select/Select";

export type ratingValueType = 0 | 1 | 2 | 3 | 4 | 5

function App() {

    const [ratingValue, setRatingValue] = useState<ratingValueType>(0)
    const [collapsed, setCollapsed] = useState<boolean>(false)
    const [switcher, setSwitcher] = useState<boolean>(false)
    const items = ['item 1', 'item 2', 'item 3', 'item 4']

    return (
        <div className={s.App}>

            <h1 style={{margin: '10px 0'}}>The Page Title</h1>

            <Accordion title={'Menu'} collapsed={collapsed} onClick={setCollapsed}/>
            <UncontrolledAccordion/>

            <Rating value={ratingValue} onClick={setRatingValue}/>
            <UncontrolledRating/>


            <OnOff isOn={switcher} onClick={setSwitcher}/>
            <UncontrolledOnOff/>

            <Select items={items}/>

        </div>
    );
}

export default App;
