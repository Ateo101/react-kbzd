import React from 'react';
import PageTitle from "./PageTitle";
import Accordion from "./components/Accordion/Accordion";
import Rating from "./components/Rating/Rating";
import OnOff from "./components/OnOff/OnOff";
import UncontrolledOnOff from "./components/OnOff/UncontrolledOnOff";
import UncontrolledAccordion from "./components/Accordion/UncontrolledAccordion";
import UncontrolledRating from "./components/Rating/UncontrolledRating";

function App() {
  return (
    <div className="App">

      <PageTitle title={'The Page Title'}/>
{/*      <Accordion title={'Menu'} collapsed={true}/>
      <Accordion title={'Menu'} collapsed={false}/>*/}
      <UncontrolledAccordion title={'Menu'}/>

{/*      <Rating value={0}/>
      <Rating value={1}/>
      <Rating value={2}/>
      <Rating value={3}/>
      <Rating value={4}/>
      <Rating value={5}/>*/}

      <UncontrolledRating/>

      {/*<OnOff isOn={true}/>*/}
      <UncontrolledOnOff/>

    </div>
  );
}

export default App;
