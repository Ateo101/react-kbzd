import Star from "./Star";
import {useState} from "react";
import s from '../../App.module.css'

const UncontrolledRating = () => {

    const [value, setValue] = useState(0)
    const values = [1, 2, 3, 4, 5]

    const onClickHandler = (v: number) => () => {
        setValue(v)
    }
    const onDoubleClickHandler = () => {
        setValue(0)
    }

    return (
        <div className={s.starContainer}>

            {/*<Star selected={value > 0} onClickCallback={onClickHandler(1)} onDoubleClickCallback={onDoubleClickHandler}/>
            <Star selected={value > 1} onClickCallback={onClickHandler(2)} onDoubleClickCallback={onDoubleClickHandler}/>
            <Star selected={value > 2} onClickCallback={onClickHandler(3)} onDoubleClickCallback={onDoubleClickHandler}/>
            <Star selected={value > 3} onClickCallback={onClickHandler(4)} onDoubleClickCallback={onDoubleClickHandler}/>
            <Star selected={value > 4} onClickCallback={onClickHandler(5)} onDoubleClickCallback={onDoubleClickHandler}/>*/}

            {values.map( (v) => <Star selected={value > v-1}
                                              onClickCallback={onClickHandler(v)}
                                              onDoubleClickCallback={onDoubleClickHandler}/> )}

        </div>
    )

}

export default UncontrolledRating;