import Star from "./Star";
import {useState} from "react";
import s from '../../App.module.css'

const UncontrolledRating = () => {

    const [value, setValue] = useState(0)
    const possibleRating = [1, 2, 3, 4, 5]

    const onClickHandler = (v: number) => () => {
        setValue(v)
    }
    const onDoubleClickHandler = () => {
        setValue(0)
    }

    return (
        <div className={s.starContainer}>
            {possibleRating.map((v) => <Star selected={value > v - 1}
                                             onClickCallback={onClickHandler(v)}
                                             onDoubleClickCallback={onDoubleClickHandler}/>)}
        </div>
    )

}

export default UncontrolledRating;