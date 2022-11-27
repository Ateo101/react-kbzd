import Star from "./Star";
import {useState} from "react";

const UncontrolledRating = () => {

    const [value,setValue] = useState(0)

    return (
        <div>
            <Star selected={value > 0} onClickCallback={() => setValue(1)} onDoubleClickCallback={() => setValue(0)}/>
            <Star selected={value > 1} onClickCallback={() => setValue(2)} onDoubleClickCallback={() => setValue(0)}/>
            <Star selected={value > 2} onClickCallback={() => setValue(3)} onDoubleClickCallback={() => setValue(0)}/>
            <Star selected={value > 3} onClickCallback={() => setValue(4)} onDoubleClickCallback={() => setValue(0)}/>
            <Star selected={value > 4} onClickCallback={() => setValue(5)} onDoubleClickCallback={() => setValue(0)}/>
        </div>
    )
}

export default UncontrolledRating;