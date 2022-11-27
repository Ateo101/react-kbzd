import React from 'react';

type PageTitlePropsType = {
    title: string;
}

const PageTitle = (props: PageTitlePropsType) => {
    return (
        <h1 style={{margin: '10px 0'}}>{props.title}</h1>
    )
}

export default PageTitle;
