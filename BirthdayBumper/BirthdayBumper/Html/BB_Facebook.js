$(document).ready( function () {        
    alert(document.URL);

    FB.getLoginStatus(function(response){
        if(response.status == 'connected') {
            FB.ui(
                {
                    method: 'feed',
                    to: 'varagrawal',
                    name: 'Facebook Dialogs',
                    link: 'https://developers.facebook.com/docs/dialogs/',
                    picture: 'http://fbrell.com/f8.jpg',
                    caption: 'Reference Documentation',
                    description: 'Dialogs provide a simple, consistent interface for applications to interface with users.'
                },
                function (response) {
                    if (response && response.post_id) {
                        alert('Post was published.');
                    } else {
                        alert('Post was not published.');
                    }
                }
            );
        } 
    });

});

function postToWall(userID) {
    
}