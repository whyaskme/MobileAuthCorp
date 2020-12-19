<div id="columnMain" style="margin-bottom:0;padding-bottom:0;">
    <div class="row980">
        <!--<p class="page-header">MAC Bank Accounts</p>-->
        <!--<p class="page-header">Accounts</p>-->
        <!--<div style="margin-top: 1rem;color:#999;text-align:center;line-height:36px;font-size:1.75rem;font-weight:bold;">ACCOUNTS</div>-->
    </div>
    <div id="main-content" class="main-content" style="min-height: 350px;">
        <div class="container">
            <div class="row" style="margin-top: 1rem;">
                <h4 class="indent-text-10" style="width:90%;"><strong>Primary Account:</strong></h4>
                <div class="col-md-12">
                    <div style="margin:0 !important;padding:0 !important;border:1px solid #b3b3b3 !important;">
                        <table id="cart-content" style="z-index:0;">
                            <thead>
                                <tr>
                                    <th>Primary Acct</th><th>Balance</th><th>Created</th><th>Updated</th><th>Holder</th><th>Login Name</th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr>
                                    <td>
                                        <div class="visible-sm visible-xs" style="margin-top:0.5rem;"><strong>Primary Account</strong></div>
                                        <?= substr($primAccount['PAN'], 0, 4).'-'.substr($primAccount['PAN'], 4, 4).'-'.substr($primAccount['PAN'], 8, 4).'-'.substr($primAccount['PAN'], 12, 4);?>
                                    </td>
                                    <td>
                                        <div class="visible-sm visible-xs" style="margin-top:0.5rem;"><strong>Balance</strong></div>
                                        <?= $primAccount['Balance'] ?>
                                    </td>
                                    <td>
                                        <div class="visible-sm visible-xs" style="margin-top:0.5rem;"><strong>Created</strong></div>
                                        <?= $primAccount['Created'] ?>
                                    </td>
                                    <td>
                                        <div class="visible-sm visible-xs" style="margin-top:0.5rem;"><strong>Updated</strong></div>
                                        <?= $primAccount['Updated'] ?>
                                    </td>
                                    <td>
                                        <div class="visible-sm visible-xs" style="margin-top:0.5rem;"><strong>Holder</strong></div>
                                        <?= $primAccount['AccountHolder'] ?>
                                    </td>
                                    <td>
                                        <div class="visible-sm visible-xs" style="margin-top:0.5rem;"><strong>Login Name</strong></div>
                                        <?= $primAccount['LoginName'] ?>
                                    </td>
                                </tr>
                            </tbody>                        
                        </table>
                    </div>
                </div>  <!-- end of col -->
            </div>  <!-- end of row -->
   
            <div class="row" style="margin-top: 1rem;margin-bottom: 0;">
                <h4 class="indent-text-10" style="width:90%;"><strong>Cards / SubAccounts:</strong></h4>
                <div class="col-md-12">
                    <div style="margin:0 !important;padding:0 !important;border:1px solid #b3b3b3 !important;">
                        <table id="cart-content">
                            <thead>
                                <tr>
                                    <th>Acct Name</th>
                                    <th>Number</th>
                                    <th>Enabled</th>
                                    <th>Last Access</th>
                                    <th>Balance</th>
                                    <th>Limit</th>
                                </tr>
                            </thead>
                            <tbody>
                                <?php
                                    if(!empty($myAccounts)) {
                                        foreach($myAccounts as $index => $account) {
			                                $formattedNumber = substr($account['Number'], 0, 4).'-'.substr($account['Number'], 4, 4).'-'.substr($account['Number'], 8, 4).'-'.substr($account['Number'], 12, 4);  
                                            echo  '
                                <tr>
                                    <td><div class="visible-sm visible-xs" style="margin-top:0.5rem;"><strong>Account Name</strong></div>'.
                                    $account['Name'].
                                    '</td>
                                    <td><div class="visible-sm visible-xs" style="margin-top:0.5rem;"><strong>Number</strong></div>'.
                                    $formattedNumber.
                                    '</td>
                                    <td><div class="visible-sm visible-xs" style="margin-top:0.5rem;"><strong>Enabled</strong></div>'.
                                    $account['Enabled'].
                                    '</td>
                                    <td><div class="visible-sm visible-xs" style="margin-top:0.5rem;"><strong>Last Access</strong></div>'. 
                                    $account['LastAccessed'].
                                    '</td>
                                    <td><div class="visible-sm visible-xs" style="margin-top:0.5rem;"><strong>Balance</strong></div><span>'.
                                    $account['Balance'].'</span>
                                    </td> 
	                                <td><div class="visible-sm visible-xs" style="margin-top:0.5rem;"><strong>Limit</strong></div>'.
	                                $account['Limit'].
	                                '</td>
      
                                </tr>';
                                    } 
                                }
                                else  {
                                    echo '<tr><td>no bills...</td></tr>';
                                }

                                ?>
                            </tbody>
                        </table>
                    </div>
                </div>   <!-- end of col -->
            </div>  <!-- end of row -->
        </div> <!--end container-->
    </div>  <!-- main content -->    
</div>
<script type="text/javascript">
    $(document).ready(function () {
        $("#footer_txt").hide();
        $("#menu_accounts").addClass("selected");
    });
</script>