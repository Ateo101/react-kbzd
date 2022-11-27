import React from 'react';

type PageTitlePropsType = {
    title: string;
}

const PageTitle = (props: PageTitlePropsType) => {
    return (
        <h1>{props.title}</h1>
    )
}

export default PageTitle;
