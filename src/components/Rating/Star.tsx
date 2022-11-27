import React from 'react';
import starGray from './star-gray.svg';
import starGlows from './star-glows.svg';
import s from '../../App.module.css'

type StarPropsType = {
    selected: boolean,
    onClickCallback?: () => void
    onDoubleClickCallback?: () => void
}

const Star = (props: StarPropsType) => {
    return (
        <span onClick={props.onClickCallback} onDoubleClick={props.onDoubleClickCallback}>
            {props.selected
                ? <img src={starGlows} className={s.star}/>
                : <img src={starGray} className={s.star}/>
            }
        </span>
    )
}

export default Star;