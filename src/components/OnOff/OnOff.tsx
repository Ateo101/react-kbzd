import React from 'react';
import s from '../../App.module.css'

type OnOffPropsType = {
    isOn: boolean;
}

const OnOff = (props: OnOffPropsType) => {



    return (
        <div>
            <div className={props.isOn ? s.on : ''}>
                On
            </div>
            <div className={!props.isOn ? s.off : ''}>
                Off
            </div>
            <div className={props.isOn ? s.on : s.off}>
                indicator
            </div>
        </div>
    )
}

export default OnOff;