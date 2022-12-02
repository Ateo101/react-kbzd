import {useState} from "react";

type AccordionTitlePropsType = {
    onClickCallback: () => void,
}
function AccordionTitle(props: AccordionTitlePropsType) {

    const title = 'Menu'

    return (
        <button onClick={props.onClickCallback}> {title} </button>
    )
}
function AccordionBody() {

    const liValues = [1,2,3]

    return (
        <ul>
            {liValues.map( v => <li> {v} </li> )}
        </ul>
    )

}

const UncontrolledAccordion = () => {

    const [collapsed, setCollapsed] = useState(false)

    return (
        <div>
            <AccordionTitle onClickCallback={()=>setCollapsed(!collapsed)}/>
            {!collapsed && <AccordionBody/>}
        </div>
    )

}

export default UncontrolledAccordion;
