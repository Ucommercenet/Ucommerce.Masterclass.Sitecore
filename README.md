# Ucommerce.Masterclass.Sitecore Day 1 completed
Ucommerce Masterclass repo for Sitecore Day 1

Remember to do the following to web.config for confirmation email to work:

1. Add to web.config:

  <system.net>
    <mailSettings>
      <smtp deliveryMethod="SpecifiedPickupDirectory">
        <specifiedPickupDirectory pickupDirectoryLocation="C:\Users\danielbergfrederikse\Desktop" />
      </smtp>
    </mailSettings>
  </system.net>

2. Change payment methods to use default payment provider, and set an acceptUrl.
