import React from 'react';
import s from '../../App.module.css'

type OnOffPropsType = {
    isOn: boolean
    onClick: (isOn: boolean) => void
}

const OnOff = (props: OnOffPropsType) => {

    const onClickHandler = (isOn: boolean) => () => {
        props.onClick(isOn)
    }

    return (
        <div>
            <button className={props.isOn ? s.indicatorBtn + ' ' + s.on : s.indicatorBtn}
                    onClick={onClickHandler(true)}>
                On
            </button>
            <button className={!props.isOn ? s.indicatorBtn + ' ' + s.off : s.indicatorBtn}
                    onClick={onClickHandler(false)}>
                Off
            </button>
            <div className={props.isOn ? s.on + ' ' + s.led : s.off + ' ' + s.led}></div>
        </div>
    )
}

export default OnOff;