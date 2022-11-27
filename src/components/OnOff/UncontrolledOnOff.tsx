import React, {useState} from 'react';
import s from '../../App.module.css'

const UncontrolledOnOff = () => {

    const [on, setOn] = useState(false)

    return (
        <div>

            <button className={on ? s.indicatorBtn + ' ' + s.on : s.indicatorBtn}
                onClick={()=>setOn(true)}>
                On
            </button>
            <button className={!on ? s.indicatorBtn + ' ' + s.off : s.indicatorBtn}
                    onClick={()=>setOn(false)}>
                Off
            </button>
            <div className={on ? s.on + ' ' + s.led : s.off + ' ' + s.led}></div>

        </div>
    )

}

export default UncontrolledOnOff;