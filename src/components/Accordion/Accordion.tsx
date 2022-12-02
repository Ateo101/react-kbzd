import React from 'react';

type AccordionPropsType = {
    title: string
    collapsed: boolean
    onClick: (collapsed: boolean) => void
}

function AccordionTitle(props: AccordionPropsType) {

    const onClickHandler = () => {
        props.onClick(!props.collapsed)
    }

    return (
        <button onClick={onClickHandler}> {props.title}</button>
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


const Accordion = (props: AccordionPropsType) => {
    return (
        <div>
            <AccordionTitle title={props.title} onClick={props.onClick} collapsed={props.collapsed}/>
            {!props.collapsed && <AccordionBody/>}
        </div>
    )
}


export default Accordion;
