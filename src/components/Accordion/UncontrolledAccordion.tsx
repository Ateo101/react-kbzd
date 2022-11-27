import {useState} from "react";

type AccordionPropsType = {
    title: string,
}

type AccordionTitlePropsType = {
    title: string,
    onClickCallback: () => void,
}

function AccordionTitle(props: AccordionTitlePropsType) {
    return (
        <button onClick={props.onClickCallback}> {props.title} </button>
    )
}

function AccordionBody() {
    return (
        <ul>
            <li>
                1
            </li>
            <li>
                2
            </li>
            <li>
                3
            </li>
        </ul>
    )
}

const UncontrolledAccordion = (props: AccordionPropsType) => {

    const [collapsed, setCollapsed] = useState(false)

    return (
        <div>
            <AccordionTitle title={props.title} onClickCallback={()=>setCollapsed(!collapsed)}/>
            {!collapsed && <AccordionBody/>}
        </div>
    )
}

export default UncontrolledAccordion;
