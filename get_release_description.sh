read -r -d '' release_description << 'EOF'
The Safe Authenticator acts as a gateway to the [Safe Network](https://safenetwork.tech/) by enabling users to create an account & authenticate themselves onto the Safe Network.
It helps users ensure they have full control over the permissions they grant to Safe apps.

## Changelog
CHANGELOG_CONTENT

## SHA-256 checksums:
```
APK checksum
APK_CHECKSUM

IPA checksum
IPA_CHECKSUM
```

## Related Links
* Safe Browser - [Desktop](https://github.com/maidsafe/sn_browser/releases/) | [Mobile](https://github.com/maidsafe/sn_mobile_browser/)
* [Safe CLI](https://github.com/maidsafe/sn_api/tree/master/sn_cli)
* [Safe Network Node](https://github.com/maidsafe/sn_node/releases/latest/)
* [sn_csharp](https://github.com/maidsafe/sn_csharp/)
EOF

apk_checksum=$(sha256sum "../net.maidsafe.SafeAuthenticator.apk" | awk '{ print $1 }')
ipa_checksum=$(sha256sum "../SafeAuthenticatoriOS.ipa" | awk '{ print $1 }')
changelog_content=$(sed '1,/]/d;/##/,$d' ../CHANGELOG.MD)
release_description=$(sed "s/APK_CHECKSUM/$apk_checksum/g" <<< "$release_description")
release_description=$(sed "s/IPA_CHECKSUM/$ipa_checksum/g" <<< "$release_description")

echo "${release_description/CHANGELOG_CONTENT/$changelog_content}" > release_description.txt
