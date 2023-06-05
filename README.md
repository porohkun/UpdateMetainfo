# UpdateMetainfo
This simple app updates pictures exifs and metadata with date taken from google photo exports

# What is this app for:
If you are downloading Google Photo Export, you may notice that the photos have the 'created date' and the modified date as today. In addition, some photos may not have the date of shooting in the exif.

This may interfere with the correct upload of photos to other photo storage services.

But along with the photos, Google creates a file like my_photo.jpg.json, which contains the date of shooting.

My application simply takes this date and writes it to the photo's 'created date', 'modified date', and exif.

# How to use:
1. Download google photo export
2. Run the application with the parameter:
   ./UpdateMetainfo c:\path\to\your\google\photo\export
