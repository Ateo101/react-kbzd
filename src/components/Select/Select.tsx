import {useState, KeyboardEvent} from "react";

type SelectPropsType = {
    items: string[],
}

const Select = (props: SelectPropsType) => {

    const [collapsed, setCollapsed] = useState(true)
    const [currItemIndex, setCurrItemIndex] = useState(0)
    const [currItem, setCurrItem] = useState(props.items[currItemIndex])
    const [hoveredItem, setHoveredItem] = useState(props.items[currItemIndex])

    const onClickCollapsed = () => setCollapsed(!collapsed)
    const collapsedOnBlur = () => setCollapsed(true)
    const onClickSetCurrItem = (name: string, index: number) => () => {
        setCurrItem(name)
        setHoveredItem(name)
        setCurrItemIndex(index)
        setCollapsed(!collapsed)
    }

    const highlightOnKeyUp = (e: KeyboardEvent<HTMLDivElement>) => {
        const btn = e.key
        if (btn === 'ArrowUp' || 'ArrowDown') {

            for (let i = currItemIndex; i < props.items.length; i++) {

                if (btn === 'ArrowUp' && currItemIndex !== 0) {
                    console.log('up')
                    setCurrItemIndex(i - 1)
                    setCurrItem(props.items[i - 1])
                    setHoveredItem(props.items[i - 1])
                    return
                } else if (btn === 'ArrowDown' && props.items[i + 1]) {
                    console.log('down')
                    setCurrItemIndex(i + 1)
                    setCurrItem(props.items[i + 1])
                    setHoveredItem(props.items[i + 1])
                    return
                }

            }

        }
    }

    return (
        <div onClick={onClickCollapsed} onKeyUp={highlightOnKeyUp} onBlur={collapsedOnBlur} tabIndex={0}>
            <b>{currItem}</b>
            <div>
                {!collapsed && props.items.map((i, index) => <div key={index}
                                                                  style={i === hoveredItem ? {fontWeight: 'bold'} : {}}
                                                                  onClick={onClickSetCurrItem(i, index)}>{i}</div>)}
            </div>
        </div>
    )

}

export default Select;
