import React, { Component } from 'react';

export class Home extends Component {
  static displayName = Home.name;

    render() {
        return (
            <div>
                <h1>Welcome to ViewBoard CRM</h1>
                <p>Manage your tasks and projects with ease!</p>
                <h2>Features:</h2>
                <ul>
                    <li>Task tracking and organization</li>
                    <li>Team collaboration and communication</li>
                    <li>Customizable workflows and boards</li>
                    <li>Automated notifications and reminders</li>
                </ul>
                <h2>Technologies Used:</h2>
                <ul>
                    <li><a href='https://get.asp.net/'>ASP.NET Core</a> and <a href='https://msdn.microsoft.com/en-us/library/67ef8sbd.aspx'>C#</a> for cross-platform server-side code</li>
                    <li><a href='https://facebook.github.io/react/'>React</a> for client-side code</li>
                    <li><a href='http://getbootstrap.com/'>Bootstrap</a> for layout and styling</li>
                </ul>
                <h2>Getting Started:</h2>
                <ul>
                    <li><strong>Register</strong> an account or <strong>log in</strong> if you already have one.</li>
                    <li><strong>Create boards</strong> to organize your tasks and projects.</li>
                    <li><strong>Add tasks</strong> to your boards and assign them to team members.</li>
                    <li><strong>Track progress</strong> and update task statuses as work is completed.</li>
                    <li><strong>Communicate</strong> with your team using comments and notifications.</li>
                </ul>
                <p>Start using your Monday CRM clone today and supercharge your productivity!</p>
            </div>
        );
    }

}
