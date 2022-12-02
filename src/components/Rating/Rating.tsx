import React from 'react';
import Star from "./Star";
import {ratingValueType} from "../../App";

type RatingPropsType = {
    value: ratingValueType
    onClick: (v: ratingValueType) => void
}

const Rating = (props: RatingPropsType) => {

    const onClickHandler = (v: ratingValueType) => () => {
        props.onClick(v)
    }
    const onDoubleClickHandler = () => {
        props.onClick(0)
    }

    return (
        <div>
            <Star selected={props.value > 0 } onClickCallback={onClickHandler(1)} onDoubleClickCallback={onDoubleClickHandler}/>
            <Star selected={props.value > 1 } onClickCallback={onClickHandler(2)} onDoubleClickCallback={onDoubleClickHandler}/>
            <Star selected={props.value > 2 } onClickCallback={onClickHandler(3)} onDoubleClickCallback={onDoubleClickHandler}/>
            <Star selected={props.value > 3 } onClickCallback={onClickHandler(4)} onDoubleClickCallback={onDoubleClickHandler}/>
            <Star selected={props.value > 4 } onClickCallback={onClickHandler(5)} onDoubleClickCallback={onDoubleClickHandler}/>
        </div>
    )
}

export default Rating;