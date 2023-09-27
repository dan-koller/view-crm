import React, { Component } from 'react';

export class Home extends Component {
    static displayName = Home.name;

    render() {
        return (
            <div className='container-fluid'>
                <div className='home text-center'>
                    <h1>Welcome to View.CRM!</h1>
                    <p>View is a CRM application that allows you to manage your customers and their orders.</p>

                    <h2>Getting started</h2>
                    <p>To get started, click on the <strong>Register</strong> link in the top right corner to create an account and login.</p>

                    <h2>Technologies</h2>
                    <p>View is built using the following technologies:</p>
                    <ul className="list-unstyled text-left">
                        <li><a href='https://get.asp.net/'>ASP.NET Core</a> and <a href='https://msdn.microsoft.com/en-us/library/67ef8sbd.aspx'>C#</a> for cross-platform server-side code</li>
                        <li><a href='https://facebook.github.io/react/'>React</a> for client-side code</li>
                        <li><a href='http://getbootstrap.com/'>Bootstrap</a> for layout and styling</li>
                    </ul>
                </div>
            </div>
        );
    }
}
