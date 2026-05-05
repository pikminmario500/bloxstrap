import glob, re
import xml.etree.ElementTree as ET

exports = input("Path of folder of exported Crowdin files: ")
dest = input("Destination resources folder: ")

icu_codes = {
	"zh-CN": "zh-Hans-CN",
	"zh-HK": "zh-Hant-HK",
	"zh-TW": "zh-Hant-TW"
}

wanted_keys = {
	"Bootstrapper.AutoUpdateFailed"
	"Common.Close",
	"Dialog.Connectivity.UnableToConnect",
	"Dialog.Exception.Report",
	"Dialog.Title"
}

for filename in glob.glob(f"{exports}\\**\\*.*", recursive=True):
	print(f"Copying {filename}")

	localeCode = re.search("\\\\([a-zA-Z\\-]+)\\\\Strings.", filename).group(1)
	localeCode = icu_codes.get(localeCode, localeCode)

	tree = ET.parse(filename)
	root = tree.getroot()

	for data in root.findall("data"):
		if data.get("name") not in wanted_keys:
			root.remove(data)

	tree.write(dest + f"\\Strings.{localeCode}.resx", encoding="unicode", xml_declaration=True)