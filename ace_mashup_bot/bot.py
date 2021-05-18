# import tweepy
from mastodon import Mastodon
import logging
import os
import random
import shutil
from config import create_api

logging.basicConfig(level=logging.INFO)
logger = logging.getLogger()

def tweet_mashup():
    logger.info("Choosing random mashup")
    # Get a mashup file path
    file = get_mashup()
    # Get file name
    name = os.path.splitext(os.path.basename(file))[0]
        # If two characters have the same last name, there may be a [counter] in the file name
    if ("[" in name):
        index = name.find("[")
        name = name[:index]
    # Grab the full character names from the file name
    index = name.find("&")
    headname = name[:index]
    bodyname = name[index + 1:]
    # Extract the first and last name from the file path
    if ("!" in headname):
        index = headname.find("!")
        firstname = headname[:index]
    if ("!" in bodyname):
        index = bodyname.find("!")
        lastname = bodyname[index + 1:]
    # Format the full names for the alt-text
    headname = headname.replace("!", " ")
    bodyname = bodyname.replace("!", " ")
    alttext = "it's " + headname + "'s head on " + bodyname + "'s body"
    # Tweet the file along with the name
    logger.info("Tweeting mashup")
    file = os.environ['MONSTROSITIES_FOLDER'] + "/" + file
    # Create an API request to do so
    hostinstance = os.environ['HOST_INSTANCE']
    token = os.environ['ACCESS_TOKEN']
    secret = os.environ['CLIENT_SECRET']
    # Login 
    mastodon = Mastodon(
    client_id = secret,
    api_base_url = hostinstance
    )
    mastodon.log_in(
    os.environ['EMAIL_ADDRESS'],
    os.environ['PASSWORD'],
    to_file = secret
    )
    #   Set up Mastodon
    mastodon = Mastodon(
    access_token = token,
    api_base_url = hostinstance
    )
    # Post
    media = mastodon.media_post(file, description=alttext)
    mastodon.status_post(firstname + " " + lastname, media_ids=media)
    # Move the file to the 'Tweeted' folder
    logger.info("Moving to 'Tweeted' folder")
    shutil.move(file, os.environ['TWEETED_FOLDER'] + "/" + name + ".png")
    # OLD TWITTER CODE
    # api.update_with_media(file, firstname + " " + lastname + " #aceattorney")


def get_mashup():
    # Select random mashup from 'Monstrosities' folder
    return random.choice(os.listdir(os.environ['MONSTROSITIES_FOLDER']))

def main():
    # OLD TWITTER CODE
    # api = create_api()
    # tweet_mashup(api)
    create_api()
    tweet_mashup()

if __name__ == "__main__":
    main()
